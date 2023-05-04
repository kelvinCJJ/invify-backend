using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Interfaces;
using Invify_API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Invify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {

        private IRepositoryWrapper _repositoryWrapper;
        //generate methods for CRUD

        public SalesController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
        }

        //predict sales
        [HttpGet("predict")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPredictSales()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindAllAsync();
                if (sales != null)
                {
                    //PredictionModel.();
                    // Load model and predict the next set values.
                    // The number of values predicted is equal to the horizon specified while training.
                    //var result = PredictionModel.Predict(sales);                   
                    return Ok();
                }
                else
                {
                    return Ok(0);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSalesAsync()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindAllAsync();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        //total revenue last month
        [HttpGet("totalrevenue/lastmonth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalRevenue()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.DateTimeCreated >= DateTime.UtcNow.AddHours(8).Date.AddMonths(-1));
                if (sales != null)
                {
                    decimal totalRevenue = 0;
                    foreach (var sale in sales)
                    {
                        totalRevenue += sale.Price;
                    }
                    return Ok(totalRevenue);

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //total revenue last week
        [HttpGet("totalrevenue/lastweek")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalRevenueLastWeek()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.DateTimeCreated >= DateTime.UtcNow.AddHours(8).Date.AddDays(-7));
                if (sales != null)
                {
                    decimal totalRevenue = 0;
                    foreach (var sale in sales)
                    {
                        totalRevenue += sale.Price;
                    }
                    return Ok(totalRevenue);

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //total gross profit last month
        [HttpGet("totalgrossprofit/lastmonth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalGrossProfit()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.DateTimeCreated >= DateTime.UtcNow.AddHours(8).Date.AddMonths(-1));
                if (sales != null)
                {
                    decimal totalGrossProfit = 0;
                    foreach (var sale in sales)
                    {
                        totalGrossProfit += sale.Price - sale.Product.Cost;
                    }
                    return Ok(totalGrossProfit);

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("lastmonth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSalesByLastMonth()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.DateTimeCreated >= DateTime.UtcNow.AddHours(8).Date.AddMonths(-1));
                if (sales != null)
                {
                    return Ok(sales);

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("lastweek")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSalesByLastWeek()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.DateTimeCreated >= DateTime.UtcNow.AddHours(8).Date.AddDays(-7));
                if (sales != null)
                {
                    return Ok(sales);

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("lastday")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSalesByLastDay()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.DateTimeCreated >= DateTime.UtcNow.AddHours(8).Date.AddDays(-1));
                if (sales != null)
                {
                    return Ok(sales);

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSalesById(int id)
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.Id == id);
                if (sales != null)
                {
                    return Ok(sales.First());

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateSales(Sale sale)
        {
            try
            {
                // proceed with create/update operation
                sale.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                var sales = await _repositoryWrapper.Sale.CreateAsync(sale);
                return Ok(sales);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while creating, please try again later" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSalesAsync([FromBody] Sale sale, int id)
        {
            sale.DateTimeUpdated = DateTime.UtcNow.AddHours(8);
            await _repositoryWrapper.Sale.UpdateAsync(sale);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSaleAsync(int id)
        {

            var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.Id == id);
            await _repositoryWrapper.Sale.DeleteAsync(sales.First());
            return Ok();
        }
    }
}

