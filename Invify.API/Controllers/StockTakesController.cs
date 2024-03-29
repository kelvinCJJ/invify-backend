﻿using Invify.Domain.Entities;
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateStockTakesAsync(List<StockTake> stockList)
        {
            try
            {
                foreach (var stock in stockList)
                {
                    var product = await _repositoryWrapper.Product.FindByConditionAsync(x => x.Id == stock.ProductId);
                    if (product == null)
                    {
                        return NotFound(new Response { Success = false, Message = "Product not found" });
                    }
                    var stockTakes = await _repositoryWrapper.StockTake.CreateAsync(stock);
                }
                return Ok(stockList);
            
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }
    }
}
