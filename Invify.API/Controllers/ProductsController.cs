using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Invify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        //generate methods for CRUD

        public ProductsController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            try
            {
                var Products = await _repositoryWrapper.Product.FindAllAsync();
                return Ok(Products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct(Product Product)
        {
            try
            {

                var isDuplicate = await _repositoryWrapper.Product.CheckForDuplicateAsync(x => x.Name == Product.Name && x.SKU==Product.SKU);
                if (isDuplicate)
                {
                    // handle duplicate case
                    return BadRequest(new Response { Success = false, Message = "Product Name/SKU already exists" });
                }
                // proceed with create/update operation
                Product.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                var res = await _repositoryWrapper.Product.CreateAsync(Product);
                return Ok(res);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while creating, please try again later" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProductAsync([FromBody] Product Product, int id)
        {
            var isDuplicate = await _repositoryWrapper.Product.CheckForDuplicateAsync(x => x.Name == Product.Name && x.SKU == Product.SKU);
            if (isDuplicate)
            {
                // handle duplicate case
                return BadRequest(new Response { Success = false, Message = "Product Name/SKU already exists" });
            }
            await _repositoryWrapper.Product.UpdateAsync(Product);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            
            var product = _repositoryWrapper.Product.FindByConditionAsync(c => c.Id == id);
            var inv = _repositoryWrapper.Inventory.FindByConditionAsync(c => c.ProductId==id);
            await _repositoryWrapper.Inventory.DeleteAsync(inv.Result.First());
            await _repositoryWrapper.Product.DeleteAsync(product.Result.First());
            return Ok();
        }
    }
}
