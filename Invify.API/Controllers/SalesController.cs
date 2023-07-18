using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML;

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

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSalesAsync()
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindAllAsync();
                var salesList = sales.Select(s => new
                {
                    s.Id,
                    s.ProductId,
                    ProductName = s.Product?.Name,
                    s.Quantity,
                    s.Price,
                    s.SaleDate
                });

                return Ok(salesList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateSales(Sale sale)
        {
            try
            {
                // proceed with create/update operation
                sale.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                var sales = await _repositoryWrapper.Sale.CreateAsync(sale);

                //product quantity update
                var product = await _repositoryWrapper.Product.FindByConditionAsync(c => c.Id == sale.ProductId);
                //check if product quantity is enough
                if (product.First().Quantity < sale.Quantity)
                {
                    return Ok(new Response { Success = false, Message = "Product quantity is not enough" });
                }
                product.First().Quantity -= sale.Quantity;
                await _repositoryWrapper.Product.UpdateAsync(product.First());

                return Ok(sales);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while creating, please try again later" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSalesAsync([FromBody] Sale sale, int id)
        {
            try
            {
                //get current sale quantity
                var currentSale = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.Id == id);
                var currentSaleQuantity = currentSale.First().Quantity;

                sale.DateTimeUpdated = DateTime.UtcNow.AddHours(8);
                await _repositoryWrapper.Sale.UpdateAsync(sale);

                //product quantity update
                var product = await _repositoryWrapper.Product.FindByConditionAsync(c => c.Id == sale.ProductId);
                //check if product quantity is enough
                if (product.First().Quantity < currentSaleQuantity - sale.Quantity)
                {
                    return Ok(new Response { Success = false, Message = "Product quantity is not enough" });
                }
                product.First().Quantity -= sale.Quantity;
                var res = await _repositoryWrapper.Product.UpdateAsync(product.First());

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while updating, please try again later" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSaleAsync(int id)
        {
            try
            {
                var sales = await _repositoryWrapper.Sale.FindByConditionAsync(c => c.Id == id);
                var res = await _repositoryWrapper.Sale.DeleteAsync(sales.First());
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while deleting, please try again later" });
            }
        }
    }
}

