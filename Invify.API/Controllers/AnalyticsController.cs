using Invify.Dtos;
using Invify.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : Controller
    {
        private IRepositoryWrapper _repositoryWrapper;
        public AnalyticsController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
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

        //demand forecast by product id
        [HttpGet("product/{id}/demand")]
        public async Task<IActionResult> GetProductDemand(int id)
        {
            try
            {
                var product = await _repositoryWrapper.Product.FindByConditionAsync(c => c.Id == id);
                if (product != null)
                {
                    var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.ProductId == id);
                    var demand = sales.Sum(x => x.Quantity);
                    return Ok(demand);
                }
                return Ok(new Response { Success = false, Message = "product does not exist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }
    }
}
