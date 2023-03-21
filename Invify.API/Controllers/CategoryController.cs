using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Interfaces;
using Invify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Invify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        //generate methods for CRUD

        public CategoryController(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("/categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            var categories = await _repositoryWrapper.Category.FindAllAsync();
            return Ok();
        }

        [HttpGet("categories/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _repositoryWrapper.Category.FindByConditionAsync(c => c.Id == id);
            return Ok();
        }

        [HttpPost("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            try
            {
                await _repositoryWrapper.Category.CreateAsync(category);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "fail", Message = "Error while registering, please try again later" });
            }
        }

        [HttpPost("categories/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] Category category, int id)
        {
            await _repositoryWrapper.Category.UpdateAsync(category);
            return Ok();
        }

        [HttpDelete("categories/{id}")]
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
