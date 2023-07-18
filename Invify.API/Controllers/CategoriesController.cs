using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Dtos.CategoryDTOs;
using Invify.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace Invify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class CategoriesController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        //generate methods for CRUD

        public CategoriesController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _repositoryWrapper.Category.GetAllCategoryAsync();
                return Ok(categories);
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
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _repositoryWrapper.Category.FindByConditionAsync(c => c.Id == id);
                if (category != null)
                {
                    return Ok(category);

                }
                return Ok(new Response { Success = false, Message = "category does not exist" });
            }
            catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            try
            {
                
                var isDuplicate = await _repositoryWrapper.Category.CheckForDuplicateAsync(x => x.Name == category.Name);
                if (isDuplicate)
                {
                    // handle duplicate case
                    return Ok(new Response { Success = false, Message = "Category already exists" });
                }
                // proceed with create/update operation
                category.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                var res = await _repositoryWrapper.Category.CreateAsync(category);
                return Ok(res);
                
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
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] Category category, int id)
        {
            try
            {
                var isDuplicate = await _repositoryWrapper.Category.CheckForDuplicateAsync(x => x.Name == category.Name);
                if (isDuplicate)
                {
                    // handle duplicate case
                    return Ok(new Response { Success = false, Message = "Category already exists" });
                }
                category.DateTimeUpdated = DateTime.UtcNow.AddHours(8);
                var res = await _repositoryWrapper.Category.UpdateAsync(category);
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
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = _repositoryWrapper.Category.FindByConditionAsync(c => c.Id == id);
                var res = await _repositoryWrapper.Category.DeleteAsync(category.Result.First());
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while deleting, please try again later" });
            }
        }
    }
}
