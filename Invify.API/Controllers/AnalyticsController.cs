using Invify.API.Helpers;
using Invify.Dtos;
using Invify.Dtos.Analytics;
using Invify.Interfaces;
using Invify_API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System.Data;
using System.Data.SqlClient;
using TorchSharp.Modules;
using static Invify_API.QuantityByProductSalesModel;
using static Microsoft.ML.DataOperationsCatalog;
using static Microsoft.ML.ForecastingCatalog;

namespace Invify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : Controller
    {
        private IRepositoryWrapper _repositoryWrapper;
        private readonly IConfiguration _configuration;
        private ForecastHelper _forecastHelper;
        //private readonly MLContext mlContext;

        public AnalyticsController(
            IRepositoryWrapper repositoryWrapper,
            IConfiguration configuration
        )
        {
            _repositoryWrapper = repositoryWrapper;
            _configuration = configuration;
            _forecastHelper = new ForecastHelper();
            //mlContext = new MLContext();

        }

        // Get total revenue by time period
        [HttpGet("totalrevenue")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalRevenueByTimePeriod(DateTime startDate, DateTime endDate)
        {
            try
            {
                var sales = await _repositoryWrapper.Sale
                    .FindByConditionAsync(s => s.DateTimeCreated >= startDate && s.DateTimeCreated < endDate);

                if (sales != null)
                {
                    decimal totalRevenue = sales.Sum(s => s.Price);
                    return Ok(totalRevenue);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        // Get total gross profit by time period
        [HttpGet("totalgrossprofit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalGrossProfitByTimePeriod(DateTime startDate, DateTime endDate)
        {
            try
            {
                var sales = await _repositoryWrapper.Sale
                    .FindByConditionAsync(s => s.DateTimeCreated >= startDate && s.DateTimeCreated < endDate);

                if (sales != null)
                {
                    decimal totalGrossProfit = sales.Sum(s => s.Price - s.Product.Cost);
                    return Ok(totalGrossProfit);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //analytics revenue by product id
        [HttpGet("product/{id}/revenue")]
        public async Task<IActionResult> GetProductRevenue(int id)
        {
            try
            {
                var product = await _repositoryWrapper.Product.FindByConditionAsync(c => c.Id == id);
                if (product != null)
                {
                    var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.ProductId == id);
                    var revenue = sales.Sum(x => x.Quantity * x.Price);
                    return Ok(revenue);
                }
                return Ok(new Response { Success = false, Message = "product does not exist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //forecast sales by month
        [HttpGet("forecastsales/month")]
        public async Task<IActionResult> ForecastTotalSalesByMonth()
        {
            try
            {
                // Create a new MLContext
                var mlContext = new MLContext();

                // Get the historical sales data from the database
                var sales = await _repositoryWrapper.Sale.FindAllAsync();
                if (sales == null)
                {
                    return BadRequest(new Response { Success = false, Message = "No sales record found" });
                }

                // Get the current year and month
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;

                // Get sales data by month from database up to the current month
                var salesByMonth = sales.Where(x => x.SaleDate.Year < currentYear || (x.SaleDate.Year == currentYear && x.SaleDate.Month <= currentMonth))
                    .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
                    .Select(x => new MonthlySaleInput
                    {
                        Year = x.Key.Year,
                        Month = x.Key.Month,
                        TotalQuantity = x.Sum(y => y.Quantity)
                    }).ToList();

                //string outputModelPath = Path.Combine(Environment.CurrentDirectory, "TrainedModelsOut\\TotalSalesByMonthModel.mlnet");
                string outputModelPath = Path.Combine(Environment.CurrentDirectory, "TrainedModels\\TotalSalesByMonthModel.zip");

                ITransformer trainedModel = mlContext.Model.Load(outputModelPath, out var modelSchema);

                //SsaForecastingTransformer trainedModel = mlContext.Model.LoadWithDataLoader(outputModelPath);

                // Create a PredictionEngine object
                TimeSeriesPredictionEngine<MonthlySaleInput, MonthlySaleOutput> predictionEngine = trainedModel.CreateTimeSeriesEngine<MonthlySaleInput, MonthlySaleOutput>(mlContext);

                //MonthlySaleInput for this month
                MonthlySaleInput input = new MonthlySaleInput
                {
                    Year = currentYear,
                    Month = currentMonth,
                    TotalQuantity = 0
                };

                // Make a prediction
                MonthlySaleOutput prediction = predictionEngine.Predict(input);

                // Get the filtered forecasted values
                List<float> filteredForecastedValues = _forecastHelper.GetFilteredForecastedValues(prediction.TotalQuantity, prediction.UpperBoundTotalQuantity);

                //// Return the forecast
                return Ok(new { actual=salesByMonth, forcasted = prediction });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //forecast sales by product id
        [HttpGet("salesforecast/product/{id}")]
        public async Task<IActionResult> ForecastSalesByMonth(int id)
        {
            try
            {
                // Get the historical sales data from the database
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.ProductId == id);
                if (sales == null)
                {
                    return BadRequest(new Response { Success = false, Message = "No sales found for the product" });
                }

                // Create a new MLContext
                var mlContext = new MLContext();

                var connectionString = _configuration.GetConnectionString("DefaultConnectionString");
                DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<ProductSaleData>();
                //string query = "SELECT YEAR(SaleDate) AS SaleYear,MONTH(SaleDate) AS SaleMonth,COUNT(*) AS TotalSales FROM Sales GROUP BY YEAR(SaleDate), MONTH(SaleDate) ORDER BY SaleYear, SaleMonth;";
                string query = "SELECT CAST([Id] as REAL), CAST([ProductId] as REAL), CAST([Quantity] as REAL), CAST([Price] as REAL), [SaleDate], [DateTimeCreated], [DateTimeUpdated], [DateTimeDeleted] FROM [dbo].[Sale] WHERE ProductId ="+id;

                DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance,
                                connectionString,
                                query);

                // Load data into IDataView
                IDataView data = loader.Load(dbSource);

                // Split into train (80%), validation (20%) sets
                TrainTestData trainValidationData = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

                // Configure the forecast estimator
                //ForecastingCatalog quantityModel


                //// Return the forecast
                return Ok("");
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //train total sales by month
        [HttpGet("trainTotalSalesByMonth")]
        public async Task<IActionResult> TrainTotalSalesByMonth()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindAllAsync();

                string outputModelPath = Path.Combine(Environment.CurrentDirectory, "TrainedModels\\TotalSalesByMonthModel.zip");

                var connectionString = _configuration.GetConnectionString("DefaultConnectionString");

                // Create a new MLContext
                var mlContext = new MLContext();

                DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<MonthlySaleInput>();
                string query = "  SELECT min([SaleDate]) as SaleDate , CAST(DATEPART(year, [SaleDate])as REAL) as [Year],  CAST(DATEPART(month, [SaleDate]) as REAL) as [Month],  CAST(SUM([Quantity])as REAL) as [TotalQuantity] FROM [dbo].[Sale]" +
                    " GROUP BY DATEPART(year, [SaleDate]), DATEPART(month, [SaleDate])ORDER BY [Year], [Month]";
                //string query = " SELECT [SaleDate] , CAST(DATEPART(year, [SaleDate])as REAL) as [Year],  CAST(DATEPART(month, [SaleDate]) as REAL) as [Month],  CAST(SUM([Quantity])as REAL) as [TotalQuantity] FROM [dbo].[Sale]" +
                //    " GROUP BY [SaleDate], DATEPART(year, [SaleDate]), DATEPART(month, [SaleDate])ORDER BY [Year], [Month], [SaleDate]";

                DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance,
                                connectionString,
                                query);

                // Load data into IDataView
                IDataView trainingData = loader.Load(dbSource);
                //IDataView data = mlContext.Data.FilterRowsByColumn(trainingData, "Year", upperBound: 1);

                // Split into train (90%), validation (10%) sets
                TrainTestData trainValidationData = mlContext.Data.TrainTestSplit(trainingData, testFraction: 0.1);

                // Configure the forecast estimator
                var pipeline = mlContext.Forecasting.ForecastBySsa(windowSize: 4, seriesLength: sales.Count(), trainSize: sales.Count(), horizon: 12,
                    outputColumnName: @"TotalQuantity", inputColumnName: @"TotalQuantity", confidenceLevel: 0.95f, confidenceUpperBoundColumn: "UpperBoundTotalQuantity", confidenceLowerBoundColumn: "LowerBoundTotalQuantity");


                // Define the pipeline
                //IEstimator<ITransformer> pipeline = mlContext.Transforms.Categorical.OneHotEncoding("Year", "Year")
                //    .Append(mlContext.Transforms.Categorical.OneHotEncoding("Month", "Month"))
                //    .Append(mlContext.Transforms.Concatenate("Features", "Year", "Month"))
                //    .Append(mlContext.Transforms.CopyColumns("Label", "TotalQuantity"))
                //    .Append(mlContext.Forecasting.ForecastBySsa(windowSize: 20, seriesLength: 58, trainSize: sales.Count(), horizon: 12, outputColumnName: @"TotalQuantity", inputColumnName: @"TotalQuantity"));                  
                //.Append(mlContext.Regression.Trainers.FastTree());

                // Train the model
                //ITransformer trainedModel = pipeline.Fit(trainingData);
                SsaForecastingTransformer trainedModel = pipeline.Fit(trainingData);

                IDataView predictions = trainedModel.Transform(trainingData);

                // Save the trained model to a file
                //DataViewSchema dataViewSchema = trainingData.Schema;

                //mlContext.Model.Save(trainedModel, dataViewSchema, outputModelPath);

                //mlContext.Model.Save(trainedModel, trainingData.Schema, outputModelPath);

                var forecastEngine = trainedModel.CreateTimeSeriesEngine<MonthlySaleInput, MonthlySaleOutput>(mlContext);
                forecastEngine.CheckPoint(mlContext, outputModelPath);

                MonthlySaleInput nextMonth = new MonthlySaleInput
                {
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    TotalQuantity = 0
                };

                MonthlySaleOutput forcastedQuantity = forecastEngine.Predict(12);

                //post processed forecast
                var (nonNegativeForecastedValues, nonNegativeUpperBoundValues, nonNegativeLowerBoundValues) = _forecastHelper.GetNonNegativeForecastedValues(forcastedQuantity.TotalQuantity, forcastedQuantity.UpperBoundTotalQuantity, forcastedQuantity.LowerBoundTotalQuantity);

                MonthlySaleOutput forcastedQuantity2 = forecastEngine.Predict(nextMonth,5);

                

                IEnumerable<string> forecastOutput =
                mlContext.Data.CreateEnumerable<MonthlySaleInput>(trainValidationData.TestSet, reuseRowObject: false)
                    .Take(7)
                    .Select((MonthlySaleInput salesData, int index) =>
                    {
                        //MonthlySaleOutput prediction = forecastEngine.Predict(salesData);\
                        MonthlySaleOutput prediction = forecastEngine.Predict();
                        return $"Month: {salesData.Month}, Year: {salesData.Year}, " +
                            $"Real Quantity: {salesData.TotalQuantity}, Forecasted Quantity: {prediction.TotalQuantity[0]}" +
                            $"UpperBound: {prediction.UpperBoundTotalQuantity[0]}";
                    });

                Console.WriteLine("Quantity Forecast");
                Console.WriteLine("---------------------");
                //Console.WriteLine(forcastedQuantity.TotalQuantity[0] + " "+ forcastedQuantity.UpperBoundTotalQuantity[0]);
                foreach (var prediction in forecastOutput)
                {
                    Console.WriteLine(prediction);
                }

                return Ok(new { forcastedQuantity, forcastedQuantity2, nonNegativeLowerBoundValues, nonNegativeForecastedValues, nonNegativeUpperBoundValues });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("forecast/{id}")]
        public async Task<IActionResult> ForecastProductSales(int id)
        {
            try
            {
                // Get the historical sales data for the specified product from the database
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(s => s.ProductId == id);
                if (sales == null)
                {
                    return BadRequest(new Response { Success = false, Message = "product does not exist" });
                }

                // Convert the sales data into a format that can be used with ML.NET
                var data = sales.Select(s => new ProductSaleData
                {
                    ProductId = (float)s.ProductId,
                    Quantity = s.Quantity,
                    Price = (float)s.Price,
                    //SaleDate = s.SaleDate
                }).ToList();

                var Sample = new ModelInput()
                { 
                    Quantity = 4,
                    //SaleDate = DateTime.Now,

                };

                var result = QuantityByProductSalesModel.Predict(Sample);



                // Return the forecasted data
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        





    }
}
