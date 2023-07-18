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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                return StatusCode(StatusCodes.Status500InternalServerError,new Response { Success = false, Message = ex.Message });
            }
        }

        //get products that are lower than restock level
        [HttpGet("lowstock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                return StatusCode(StatusCodes.Status500InternalServerError,new Response { Success = false, Message = ex.Message });
            }
        }

        //get top 5 products by sales
        [HttpGet("top5products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                return StatusCode(StatusCodes.Status500InternalServerError,new Response { Success = false, Message = ex.Message });
            }
        }

        
        // get product profitability
        [HttpGet("productprofitability")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductProfitability()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindAllAsync();
                var products = await _repositoryWrapper.Product.FindAllAsync();

                var productProfitability = products.Select(p => new
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    TotalRevenue = sales.Where(s => s.ProductId == p.Id).Sum(s => s.Price * s.Quantity),
                    TotalCost = sales.Where(s => s.ProductId == p.Id).Sum(s => p.Cost * s.Quantity),
                    Profit = sales.Where(s => s.ProductId == p.Id).Sum(s => (s.Price - p.Cost) * s.Quantity)
                });

                return Ok(productProfitability);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,new Response { Success = false, Message = ex.Message });
            }
        }

        // get inventory management
        [HttpGet("inventorymanagement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInventoryManagement()
        {
            try
            {
                var purchases = await _repositoryWrapper.Purchase.FindAllAsync();
                var sales = await _repositoryWrapper.Sale.FindAllAsync();
                var stockTakes = await _repositoryWrapper.StockTake.FindAllAsync();
                var products = await _repositoryWrapper.Product.FindAllAsync();

                var inventoryManagement = products.Select(p => new
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    QuantityPurchased = purchases.Where(x => x.ProductId == p.Id).Sum(x => x.Quantity),
                    QuantitySold = sales.Where(x => x.ProductId == p.Id).Sum(x => x.Quantity),
                    QuantityTaken = stockTakes.Where(x => x.ProductId == p.Id).Sum(x => x.TakenQuantity),
                    CurrentInventoryLevel = p.Quantity
                });

                return Ok(inventoryManagement);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,new Response { Success = false, Message = ex.Message });
            }
        }

        // get sales trends this year
        [HttpGet("salestrends")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSalesTrendsThisYear()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindAllAsync();

                var salesTrendsByMonth = sales
                    .Where(x => x.SaleDate.Year == DateTime.Now.Year && x.SaleDate.Month <= DateTime.Now.Month)
                    .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
                    .Select(s => new { TotalSales = s.Sum(x => x.Quantity), TotalRevenue = s.Sum(x => x.Quantity * x.Price) })
                    .ToArray();

                // Create separate arrays for total sales and total revenue
                var totalSales = salesTrendsByMonth.Select(x => x.TotalSales).ToArray();
                var totalRevenue = salesTrendsByMonth.Select(x => (int)x.TotalRevenue).ToArray();

                return Ok(new { totalSales, totalRevenue });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,new Response { Success = false, Message = ex.Message });
            }
        }


        //forecast sales by month
        [HttpGet("forecastsales/month")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                    return Ok(new Response { Success = false, Message = "No sales record found" });
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

                string outputModelPath = Path.Combine(Environment.CurrentDirectory, "TrainedModels","TotalSalesByMonthModel.zip");

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
                
                //post processed forecast
                var (nonNegativeForecastedValues, nonNegativeUpperBoundValues, nonNegativeLowerBoundValues) = _forecastHelper.GetNonNegativeForecastedValues(prediction.TotalQuantity, prediction.UpperBoundTotalQuantity, prediction.LowerBoundTotalQuantity);

                //// Return the forecast
                return Ok(new { actualData=salesByMonth, forecastedData = nonNegativeForecastedValues });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,new Response { Success = false, Message = ex.Message });
            }
        }

        //train total sales by month
        [HttpGet("trainTotalSalesByMonth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TrainTotalSalesByMonth()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindAllAsync();

                string outputModelPath = Path.Combine(Environment.CurrentDirectory, "TrainedModels","TotalSalesByMonthModel.zip");

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


                // Train the model
                //ITransformer trainedModel = pipeline.Fit(trainingData);
                SsaForecastingTransformer trainedModel = pipeline.Fit(trainingData);

                IDataView predictions = trainedModel.Transform(trainingData);

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
                return StatusCode(StatusCodes.Status500InternalServerError,new Response { Success = false, Message = ex.Message });
            }
        }

        





    }
}
