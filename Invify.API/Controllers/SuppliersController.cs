using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;

        //get all suppliers
        //get all suppliers
        //get suppliers by name

        public SuppliersController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
        }

        //get all suppliers
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSuppliersAsync()
        {
            try
            {
                var suppliers = await _repositoryWrapper.Supplier.FindAllAsync();
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Internal error, please try again later" });
            }
        }

        //get suppliers by name
        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSupplierByName(string name)
        {
            try
            {
                var supplier = await _repositoryWrapper.Supplier.FindByConditionAsync(c => c.Name == name);
                if (supplier != null)
                {
                    return Ok(supplier.First());

                }
                return BadRequest(new Response { Success = false, Message = "supplier does not exist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //create supplier
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateSupplier([FromBody] Supplier supplier)
        {
            try
            {
                if (supplier == null)
                {
                    return BadRequest(new Response { Success = false, Message = "supplier object is null" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new Response { Success = false, Message = "Invalid supplier object" });
                }
                await _repositoryWrapper.Supplier.UpdateAsync(supplier);
                await _repositoryWrapper.SaveAsync();
                return CreatedAtRoute("SupplierById", new { id = supplier.Id }, supplier);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //update supplier
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] Supplier supplier)
        {
            try
            {
                if (supplier == null)
                {
                    return BadRequest(new Response { Success = false, Message = "supplier object is null" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new Response { Success = false, Message = "Invalid supplier object" });
                }
                var dbSupplier = await _repositoryWrapper.Supplier.FindByConditionAsync(c => c.Id == id);
                if (dbSupplier == null)
                {
                    return BadRequest(new Response { Success = false, Message = "supplier does not exist" });
                }
                await _repositoryWrapper.Supplier.UpdateAsync(supplier);
                await _repositoryWrapper.SaveAsync();
                return Ok(new Response { Success = true, Message = "supplier updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }

        //delete supplier
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            try
            {
                var supplier = await _repositoryWrapper.Supplier.FindByConditionAsync(c => c.Id == id);
                if (supplier == null)
                {
                    return BadRequest(new Response { Success = false, Message = "supplier does not exist" });
                }
                await _repositoryWrapper.Supplier.DeleteAsync(supplier.First());
                await _repositoryWrapper.SaveAsync();
                return Ok(new Response { Success = true, Message = "supplier ["+supplier.First().Name+"] deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response { Success = false, Message = ex.Message });
            }
        }


    }
}
