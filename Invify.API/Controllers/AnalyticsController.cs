using Invify.API.Helpers;
using Invify.Dtos;
using Invify.Dtos.Analytics;
using Invify.Dtos.Sale;
using Invify.Interfaces;
using Invify_API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System.Data;
using System.Data.SqlClient;
using Tensorflow;
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
        private readonly MLContext mlContext;

        public AnalyticsController(
            IRepositoryWrapper repositoryWrapper,
            IConfiguration configuration
        )
        {
            _repositoryWrapper = repositoryWrapper;
            _configuration = configuration;
            _forecastHelper = new ForecastHelper();
            mlContext = new MLContext();

        }

        //get total sales by time period
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetTotalSalesAndRevenueThisYear()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindAllAsync();
                var totalProducts = await _repositoryWrapper.Product.TotalCountAsync();

                if (sales != null)
                {
                    //total sales this year
                    var totalSalesThisYear = sales.Where(x => x.SaleDate.Year == DateTime.Now.Year).Sum(x => x.Quantity);

                    //total sales last month
                    var totalSalesLastMonth = sales.Where(x => x.SaleDate.Month == DateTime.Now.Month - 1).Sum(x => x.Quantity);

                    //total sales last week
                    var totalSalesLastWeek = sales.Where(x => x.SaleDate.DayOfYear >= DateTime.Now.DayOfYear - 7).Sum(x => x.Quantity); 
                    
                    //total revenue this year
                    var totalRevenueThisYear = sales.Where(x => x.SaleDate.Year == DateTime.Now.Year).Sum(x => x.Price * x.Quantity);

                    //total revenue last month
                    var totalRevenueLastMonth = sales.Where(x => x.SaleDate >= DateTime.Now.AddMonths(-1)).Sum(x => x.Price * x.Quantity);

                    //total revenue last week
                    var totalRevenueLastWeek = sales.Where(x => x.SaleDate > DateTime.Now.AddDays(-7)).Sum(x => x.Price * x.Quantity);

                    return Ok(new { totalRevenueThisYear, totalRevenueLastMonth, totalRevenueLastWeek, totalSalesThisYear, totalSalesLastMonth, totalSalesLastWeek, totalProducts });
                }


                return Ok(new { totalProduct=totalProducts });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //get products that are lower than restock level
        [HttpGet("lowstock")]
        public async Task<IActionResult> GetLowStockProducts()
        {
            try
            {
                var products = await _repositoryWrapper.Product.FindAllAsync();

                if (products != null)
                {
                    var lowStockProducts = products.Where(x => x.Quantity < x.RestockLevel).ToList();

                    return Ok(lowStockProducts);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //get top 5 products by sales
        [HttpGet("top5products")]
        public async Task<IActionResult> GetTop5Products()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindAllAsync();
                var products = await _repositoryWrapper.Product.FindAllAsync();

                if (sales != null)
                {

                    //top 5 products by sales this year select product name
                    var top5ProductsThisYear = sales.Where(x => x.SaleDate.Year == DateTime.Now.Year)
                        .GroupBy(x => x.ProductId)
                        .Select(x => new { ProductName = products.Where(p => p.Id == x.Key).FirstOrDefault().Name, Quantity = x.Sum(s => s.Quantity) })
                        .OrderByDescending(x => x.Quantity)
                        .Take(5);


                    //top 5 products by sales last month
                    var top5ProductsLastMonth = sales.Where(x => x.SaleDate >= DateTime.Now.AddMonths(-1) && x.SaleDate <= DateTime.Now)
                        .GroupBy(x => x.ProductId)
                        .Select(x => new { ProductName = products.Where(p => p.Id == x.Key).FirstOrDefault().Name, Quantity = x.Sum(s => s.Quantity) })
                        .OrderByDescending(x => x.Quantity)
                        .Take(5);

                    //top 5 products by sales last week
                    var top5ProductsLastWeek = sales.Where(x => x.SaleDate >= DateTime.Now.AddDays(-7)  && x.SaleDate <= DateTime.Now)
                        .GroupBy(x => x.ProductId)
                        .Select(x => new { ProductName = products.Where(p => p.Id == x.Key).FirstOrDefault().Name, Quantity = x.Sum(s => s.Quantity) })
                        .OrderByDescending(x => x.Quantity)
                        .Take(5);

                    //format data into an array of names and an array of quantities
                    var top5ProductsThisYearNames = top5ProductsThisYear.Select(x => x.ProductName).ToArray();
                    var top5ProductsThisYearQuantities = top5ProductsThisYear.Select(x => x.Quantity).ToArray();

                    var top5ProductsLastMonthNames = top5ProductsLastMonth.Select(x => x.ProductName).ToArray();
                    var top5ProductsLastMonthQuantities = top5ProductsLastMonth.Select(x => x.Quantity).ToArray();

                    var top5ProductsLastWeekNames = top5ProductsLastWeek.Select(x => x.ProductName).ToArray();
                    var top5ProductsLastWeekQuantities = top5ProductsLastWeek.Select(x => x.Quantity).ToArray();

                    var top5ProductsThisYearData = new { labelData = top5ProductsThisYearNames, seriesData = top5ProductsThisYearQuantities };
                    var top5ProductsLastMonthData = new { labelData = top5ProductsLastMonthNames, seriesData = top5ProductsLastMonthQuantities };
                    var top5ProductsLastWeekData = new { labelData = top5ProductsLastWeekNames, seriesData = top5ProductsLastWeekQuantities };
                    
                
                    return Ok (new { top5ProductsThisYearData, top5ProductsLastMonthData, top5ProductsLastWeekData });
                }

                return Ok();
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
                //var mlContext = new MLContext();

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
                int[] salesByMonth = sales
                    .Where(x => x.SaleDate.Year == currentYear && x.SaleDate.Month <= currentMonth)
                    .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
                    .Select(x => x.Sum(y => y.Quantity))
                    .ToArray();


                //get how many months left to forecast
                int monthsLeft = 12 - salesByMonth.Length;


                //string outputModelPath = Path.Combine(Environment.CurrentDirectory, "TrainedModelsOut\\TotalSalesByMonthModel.mlnet");
                string outputModelPath = Path.Combine(Environment.CurrentDirectory, "TrainedModels\\TotalSalesByMonthModel.zip");

                ITransformer trainedModel = mlContext.Model.Load(outputModelPath, out var modelSchema);

                // Create a PredictionEngine object
                TimeSeriesPredictionEngine<MonthlySaleInput, MonthlySaleOutput> predictionEngine = trainedModel.CreateTimeSeriesEngine<MonthlySaleInput, MonthlySaleOutput>(mlContext);

                //MonthlySaleInput for this month
                MonthlySaleInput input = new MonthlySaleInput
                {
                    SaleDate = DateTime.Now,
                    Year = currentYear,
                    Month = currentMonth,
                    TotalQuantity = 0
                };

                // Make a prediction
                MonthlySaleOutput prediction = predictionEngine.Predict(12);

                // Get the filtered forecasted values
                //List<float> filteredForecastedValues = _forecastHelper.GetFilteredForecastedValues(prediction.TotalQuantity, prediction.UpperBoundTotalQuantity);
                
                //post processed forecast
                var (nonNegativeForecastedValues, nonNegativeUpperBoundValues, nonNegativeLowerBoundValues) = _forecastHelper.GetNonNegativeForecastedValues(prediction.TotalQuantity, prediction.UpperBoundTotalQuantity, prediction.LowerBoundTotalQuantity);

                //// Return the forecast
                return Ok(new { actualData=salesByMonth, forecastedData = nonNegativeForecastedValues });
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
                var pipeline = mlContext.Forecasting.ForecastBySsa(windowSize: 5, seriesLength: sales.Count(), trainSize: sales.Count(), horizon: 12,
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

                MonthlySaleOutput forecastedQuantity = forecastEngine.Predict(12);

                //post processed forecast
                var (nonNegativeForecastedValues, nonNegativeUpperBoundValues, nonNegativeLowerBoundValues) = _forecastHelper.GetNonNegativeForecastedValues(forecastedQuantity.TotalQuantity, forecastedQuantity.UpperBoundTotalQuantity, forecastedQuantity.LowerBoundTotalQuantity);

                MonthlySaleOutput forecastedQuantity2 = forecastEngine.Predict(nextMonth,5);

                

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

                string forecastOutputString = "Quantity Forecast\n\"---------------------\n" + string.Join("\n\n", forecastOutput);
                //Console.WriteLine("Quantity Forecast");
                //Console.WriteLine("---------------------");
                //Console.WriteLine(forcastedQuantity.TotalQuantity[0] + " "+ forcastedQuantity.UpperBoundTotalQuantity[0]);
                //foreach (var prediction in forecastOutput)
                //{
                //    Console.WriteLine(prediction);
                //}

                return Ok(new { forecastOutputString, forecastedQuantity, forecastedQuantity2, nonNegativeLowerBoundValues, nonNegativeForecastedValues, nonNegativeUpperBoundValues });
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
