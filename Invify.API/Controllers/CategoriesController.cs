using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Dtos.CategoryDTOs;
using Invify.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace Invify.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
    
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

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var category = await _repositoryWrapper.Category.GetCategoryByNameAsync(name);
            if (category == null)
            {
                return BadRequest(ModelState);
            }
            else
            {
                return Ok(category);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _repositoryWrapper.Category.FindByConditionAsync(c => c.Id == id);
            if (category == null)
            {
                return BadRequest(ModelState);
            }
            else
            {
                return Ok(category);
            }
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            try
            {
                
                var isDuplicate = await _repositoryWrapper.Category.CheckForDuplicateAsync(x => x.Name == category.Name);
                if (isDuplicate)
                {
                    // handle duplicate case
                    return BadRequest(new Response { Success = false, Message = "Category already exists" });
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] Category category, int id)
        {
            var isDuplicate = await _repositoryWrapper.Category.CheckForDuplicateAsync(x => x.Name == category.Name);
            if (isDuplicate)
            {
                // handle duplicate case
                return BadRequest(new Response { Success = false, Message = "Category already exists" });
            }
            await _repositoryWrapper.Category.UpdateAsync(category);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            var category = _repositoryWrapper.Category.FindByConditionAsync(c => c.Id == id);
            await _repositoryWrapper.Category.DeleteAsync(category.Result.First());
            return Ok();
        }
    }
}
