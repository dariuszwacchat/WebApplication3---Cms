using Data.Repos;
using Data.Repos.Abs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize (Roles = "Administrator")]
    [Route ("api/[controller]")]
    [ApiController]
    public class CategoriesController: ControllerBase
    {
        private readonly IModelRepository <Category> _categoriesRepository;

        public CategoriesController (IModelRepository<Category> categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

         
        [HttpGet]
        public async Task<ActionResult<TaskResult<List<Category>>>> GetCategories ()
        {
            try
            {
                var taskResult = await _categoriesRepository.GetAll();
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet ("{categoryId}")]
        public async Task<ActionResult<TaskResult<Category>>> GetCategory (string categoryId)
        {
            try
            {
                var taskResult = await _categoriesRepository.Get(categoryId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost]
        public async Task<ActionResult<TaskResult<Category>>> PostCategory (Category model)
        {
            try
            {
                var taskResult = await _categoriesRepository.Create(model);
                return CreatedAtAction (nameof (GetCategory), new { categoryId = model.CategoryId }, taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPut ("{categoryId}")]
        public async Task<ActionResult<TaskResult<Category>>> PutCategory (string categoryId, Category model)
        {
            try
            {
                if (categoryId != model.CategoryId)
                    return BadRequest ("CategoryId mismatch");

                var taskResult = await _categoriesRepository.Update(model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpDelete ("{categoryId}")]
        public async Task<ActionResult<TaskResult<Category>>> DeleteCategory (string categoryId)
        {
            try
            {
                var taskResult = await _categoriesRepository.Delete(categoryId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }


    }
}