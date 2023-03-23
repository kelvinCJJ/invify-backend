using AutoMapper;
using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Dtos.Category;
using Invify.Infrastructure.Configuration;
using Invify.Interfaces;
using Invify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invify.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        //generate methods for CRUD

        public CategoryController(
            IRepositoryWrapper repositoryWrapper
        )
        {
            _repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("/categories")]
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

        [HttpGet("categories/{id}")]
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

        [HttpPost("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCategory(CategoryDTO category)
        {
            try
            {
                var cat = new Category
                {
                    Name = category.Name,
                    DateTimeCreated = DateTime.UtcNow.AddHours(8)                    
                };
                await _repositoryWrapper.Category.CreateAsync(cat);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while registering, please try again later" });
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
