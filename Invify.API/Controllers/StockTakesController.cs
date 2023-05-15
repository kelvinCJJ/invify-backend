using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Invify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockTakesController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        //generate methods for CRUD

        public StockTakesController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllStockTakesAsync()
        {
            try
            {
                var stockTakes = await _repositoryWrapper.StockTake.FindAllAsync();
                return Ok(stockTakes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStockTakesAsync(StockTake stock)
        {
            try
            {
                var product = await _repositoryWrapper.Product.FindByConditionAsync(x => x.Id == stock.ProductId);
                if (product == null)
                {
                    //await _repositoryWrapper.Product.CreateAsync(new Product { Id = stock.ProductId, Name = stock.ProductName, Quantity = stock.Quantity });
                    return NotFound(new Response { Success = false, Message = "Product not found" });
                }
                var stockTakes = await _repositoryWrapper.StockTake.CreateAsync(stock);
                return Ok(stockTakes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }
    }
}
