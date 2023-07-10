using Invify.Application.Dtos.Product;
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

        public ProductsController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            try
            {
                var products = await _repositoryWrapper.Product.FindAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        //get product id and names
        [HttpGet("idandname")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductIdandName()
        {
            try
            {
                var product = await _repositoryWrapper.Product.FindAllAsync();
                if (product != null)
                {
                    return Ok(product.Select(x => new { x.Id, x.Name }));

                }
                return BadRequest(new Response { Success = false, Message = "product does not exist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //get product by sku
        [HttpGet("sku/{sku}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductIdandNameBySKU(string sku)
        {
            try
            {
                var product = await _repositoryWrapper.Product.FindByConditionAsync(c => c.SKU == sku);
                if (product != null && product.Count>0)
                {
                    return Ok(new { product.First().Id, product.First().Name });

                }
                return Ok(new Response { Success = false, Message = "product does not exist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            try
            {

                var isDuplicate = await _repositoryWrapper.Product.CheckForDuplicateAsync(x => x.Name == product.Name && x.SKU==product.SKU);
                if (isDuplicate)
                {
                    // handle duplicate case
                    return Ok(new Response { Success = false, Message = "Product Name/SKU already exists" });
                }
                // proceed with create/update operation
                product.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                var res = await _repositoryWrapper.Product.CreateAsync(product);
                return Ok(res);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Success = false, Message = "Error while creating, please try again later" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductAsync([FromBody] Product product, int id)
        {
            product.DateTimeUpdated= DateTime.UtcNow.AddHours(8);
            await _repositoryWrapper.Product.UpdateAsync(product);
            return Ok();
        }

        //update product stock take
        [HttpPut("stocktake")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductStockTakeAsync([FromBody] List<ProductStockTakeDto> productStockTakeDtos)
        {
            try
            {
                foreach (var productStockTakeDto in productStockTakeDtos)
                {
                    var product = await _repositoryWrapper.Product.FindByConditionAsync(c => c.Id == productStockTakeDto.ProductId);
                    if (product != null && product.Count > 0)
                    {
                        product.First().Quantity += productStockTakeDto.StockTakeQuantity != null ? productStockTakeDto.StockTakeQuantity.Value : 0;
                        product.First().DateTimeUpdated = DateTime.UtcNow.AddHours(8);

                        //stock take record
                        var stockTake = new StockTake
                        {
                            ProductId = productStockTakeDto.ProductId,
                            TakenQuantity = productStockTakeDto.StockTakeQuantity != null ? productStockTakeDto.StockTakeQuantity.Value : 0,
                            DateTimeCreated = DateTime.UtcNow.AddHours(8)
                        };

                        await _repositoryWrapper.Product.UpdateAsync(product.First());
                        await _repositoryWrapper.StockTake.CreateAsync(stockTake);
                    }
                    else
                    {
                        return BadRequest(new Response { Success = false, Message = "product does not exist" });
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            
            var product = await _repositoryWrapper.Product.FindByConditionAsync(c => c.Id == id);
            await _repositoryWrapper.Product.DeleteAsync(product.First());
            return Ok();
        }
    }
}
