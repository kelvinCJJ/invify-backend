using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Invify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        //generate methods for CRUD

        public InventoriesController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllInventoriesAsync()
        {
            try
            {
                var inventories = await _repositoryWrapper.Inventory.FindAllAsync();
                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInventoriesById(int id)
        {
            try
            {
                var inventory = await _repositoryWrapper.Inventory.FindByConditionAsync(c => c.Id == id);
                if (inventory != null)
                {
                    return Ok(inventory);

                }
                return BadRequest(new Response { Success = false, Message = "inventory does not exist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateInventories(Inventory inventory)
        {
            try
            {

                var isDuplicate = await _repositoryWrapper.Inventory.CheckForDuplicateAsync(x => x.ProductId == inventory.ProductId);
                if (isDuplicate)
                {
                    // handle duplicate case
                    return BadRequest(new Response { Success = false, Message = "Product inventory already exists" });
                }
                // proceed with create/update operation
                inventory.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                var res = await _repositoryWrapper.Inventory.CreateAsync(inventory);
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
        public async Task<IActionResult> UpdateInventoryAsync([FromBody] Inventory inventory, int id)
        {
            
            inventory.DateTimeUpdated = DateTime.UtcNow.AddHours(8);
            await _repositoryWrapper.Inventory.UpdateAsync(inventory);
            return Ok();
        }

    }
}
