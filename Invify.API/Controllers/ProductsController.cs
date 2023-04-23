using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Dtos.Product;
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

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            try
            {
                var products = await _repositoryWrapper.Product.FindAllAsync();
                //var productsDTO = new List<ProductDTO>();

                //products.ForEach(product =>
                //{
                //    productsDTO.Add(
                //        new ProductDTO
                //        {
                //            Id = product.Id,
                //            Name = product.Name,
                //            SKU = product.SKU,
                //            Description = product.Description,
                //            Cost = product.Cost,
                //            Price = product.Price,
                //        }
                //        );

                //});
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _repositoryWrapper.Product.FindByConditionAsync(c => c.Id == id);
                if (product != null)
                {
                    return Ok(product.First());

                }
                return BadRequest(new Response { Success = false, Message = "product does not exist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            try
            {

                var isDuplicate = await _repositoryWrapper.Product.CheckForDuplicateAsync(x => x.Name == product.Name && x.SKU==product.SKU);
                if (isDuplicate)
                {
                    // handle duplicate case
                    return BadRequest(new Response { Success = false, Message = "Product Name/SKU already exists" });
                }
                // proceed with create/update operation
                product.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                var res = await _repositoryWrapper.Product.CreateAsync(product);
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
        public async Task<IActionResult> UpdateProductAsync([FromBody] Product product, int id)
        {
            product.DateTimeUpdated= DateTime.UtcNow.AddHours(8);
            await _repositoryWrapper.Product.UpdateAsync(product);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            
            var product = await _repositoryWrapper.Product.FindByConditionAsync(c => c.Id == id);
            var inv = await _repositoryWrapper.Inventory.FindByConditionAsync(c => c.ProductId==id);
            if (inv.Count != 0)
            {
                await _repositoryWrapper.Inventory.DeleteAsync(inv.First());
            }
            await _repositoryWrapper.Product.DeleteAsync(product.First());
            return Ok();
        }
    }
}
