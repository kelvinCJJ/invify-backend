using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Invify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;

        public PurchasesController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPurchasesAsync()
        {
            try
            {
                var purchases = await _repositoryWrapper.Purchase.FindAllAsync();
                return Ok(purchases);
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
        public async Task<IActionResult> CreatePurchaseAsync(Purchase purchase)
        {
            try
            {
                var product = await _repositoryWrapper.Product.FindByConditionAsync(x => x.Id == purchase.ProductId);
                if (product == null)
                {
                    return NotFound(new Response { Success = false, Message = "Product not found" });
                }

                purchase.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                var purchases = await _repositoryWrapper.Purchase.CreateAsync(purchase);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePurchaseAsync(int id, Purchase purchase)
        {
            try
            {
                var product = await _repositoryWrapper.Product.FindByConditionAsync(x => x.Id == purchase.ProductId);
                if (product == null)
                {
                    return NotFound(new Response { Success = false, Message = "Product not found" });
                }
                var purchases = await _repositoryWrapper.Purchase.UpdateAsync(purchase);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePurchaseAsync(int id)
        {
            try
            {
                var purchase = await _repositoryWrapper.Purchase.FindByConditionAsync(x => x.Id == id);
                if (purchase == null)
                {
                    return NotFound(new Response { Success = false, Message = "Purchase not found" });
                }
                await _repositoryWrapper.Purchase.DeleteAsync(purchase.First());
                return Ok(new Response { Success = true, Message = "Purchase deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

    }
}
