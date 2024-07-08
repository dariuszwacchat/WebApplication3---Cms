using Data.Repos.Abs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    //[Authorize (Roles = "Administrator")]
    [Route ("api/[controller]")]
    [ApiController]
    public class SubsubcategoriesController: ControllerBase
    {
        private readonly IModelRepository <Subsubcategory> _subsubcategoriesRepository;

        public SubsubcategoriesController (IModelRepository<Subsubcategory> subsubcategoriesRepository)
        {
            _subsubcategoriesRepository = subsubcategoriesRepository;
        }

        [HttpGet]
        public async Task<ActionResult<TaskResult<List<Subsubcategory>>>> GetSubsubategories ()
        {
            try
            {
                var taskResult = await _subsubcategoriesRepository.GetAll();
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet ("{subsubcategoryId}")]
        public async Task<ActionResult<TaskResult<Subsubcategory>>> GetSubsubategory (string subsubcategoryId)
        {
            try
            {
                var taskResult = await _subsubcategoriesRepository.Get(subsubcategoryId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost]
        public async Task<ActionResult<TaskResult<Subsubcategory>>> PostSubsubategory (Subsubcategory model)
        {
            try
            {
                var taskResult = await _subsubcategoriesRepository.Create(model);
                return CreatedAtAction (nameof (GetSubsubategory), new { subsubcategoryId = model.SubsubcategoryId }, taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPut ("{subsubcategoryId}")]
        public async Task<ActionResult<TaskResult<Subsubcategory>>> PutSubsubategory (string subsubcategoryId, Subsubcategory model)
        {
            try
            {
                if (subsubcategoryId != model.SubsubcategoryId)
                    return BadRequest ("SubsubcategoryId mismatch");

                var taskResult = await _subsubcategoriesRepository.Update(model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpDelete ("{subsubcategoryId}")]
        public async Task<ActionResult<TaskResult<Subsubcategory>>> DeleteSubsubategory (string subsubcategoryId)
        {
            try
            {
                var taskResult = await _subsubcategoriesRepository.Delete(subsubcategoryId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
