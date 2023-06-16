//using Invify.Domain.DTO;
//using Invify.Domain.Entities;
//using Invify.Dtos;
//using Invify.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace Invify.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class InventoriesController : ControllerBase
//    {
//        private IRepositoryWrapper _repositoryWrapper;
//        //generate methods for CRUD

//        public InventoriesController(
//            IRepositoryWrapper repositoryWrapper
//        )
//        {
//            _repositoryWrapper = repositoryWrapper;
//        }

//        //get all inventories
//        [HttpGet("")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> GetAllInventoriesAsync()
//        {
//            try
//            {
//                var inventories = await _repositoryWrapper.Inventory.FindAllAsync();
//                return Ok(inventories);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
//            }
//        }

//        //get inventory by id
//        [HttpGet("{productId}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> GetInventoriesById(int productId)
//        {
//            try
//            {
//                var inventory = await _repositoryWrapper.Inventory.FindByConditionAsync(c => c.ProductId == productId);
//                if (inventory != null)
//                {
//                    return Ok(inventory.First());

//                }
//                return Ok(new Response { Success = false, Message = "inventory does not exist" });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new Response { Success = false, Message = ex.Message });
//            }
//        }

//        //create inventory
//        [HttpPost("")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> CreateInventories([FromBody] List<StockTakeItem> items)
//        {
//            try
//            {
//                foreach (var item in items)
//                {
//                    var existingInventory = await _repositoryWrapper.Inventory.FindByConditionAsync(c => c.ProductId == item.Id);
//                    if (existingInventory != null && existingInventory.Count > 0)
//                    {
//                        // handle duplicate case
//                        return await UpdateInventoryStocktakeAsync(item);
//                    }
//                    // proceed with create operation
//                    var inventory = new Inventory();
//                    inventory.ProductId = item.Id;
//                    inventory.Quantity = item.Quantity;
//                    inventory.DateTimeCreated = DateTime.UtcNow.AddHours(8);
//                    var res = await _repositoryWrapper.Inventory.CreateAsync(inventory);
//                    return Ok(res);
//                }
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while creating, please try again later" });
//            }
//        }

//        //update inventory
//        [HttpPut]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> UpdateInventoryAsync([FromBody] Inventory inventory)
//        {
//            try
//            {
//                var searchInventory = await _repositoryWrapper.Inventory.FindByConditionAsync(c => c.ProductId == inventory.ProductId);
//                if (!searchInventory.Any())
//                {
//                    return BadRequest(new Response { Success = false, Message = "Inventory does not exist" });
//                }
//                var existingInventory = searchInventory.First();
//                existingInventory.Quantity = inventory.Quantity;
//                existingInventory.DateTimeUpdated = DateTime.UtcNow.AddHours(8);
//                await _repositoryWrapper.Inventory.UpdateAsync(existingInventory);
//                return Ok(new Response { Success = true, Message = "Inventory updated!" });

//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while updating, please try again later" });
//            }
//        }


//        //update inventory stocktake
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> UpdateInventoryStocktakeAsync([FromBody] StockTakeItem stockTakeItem)
//        {
//            try
//            {
//                var searchInventory = await _repositoryWrapper.Inventory.FindByConditionAsync(c => c.ProductId == stockTakeItem.Id);
//                if (!searchInventory.Any())
//                {
//                    return BadRequest(new Response { Success = false, Message = "Inventory does not exist" });
//                }
//                var inventory = searchInventory.First();
//                inventory.Quantity += stockTakeItem.Quantity;
//                inventory.DateTimeUpdated = DateTime.UtcNow.AddHours(8);
//                await _repositoryWrapper.Inventory.UpdateAsync(inventory);
//                return Ok(new Response { Success = true, Message = "Inventory updated!" });

//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while updating, please try again later" });
//            }
//        }
//    }
//}
