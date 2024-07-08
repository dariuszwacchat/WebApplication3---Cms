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
    public class PhotoProductsController: ControllerBase
    {
        private readonly IModelRepository <PhotoProduct> _photoProductsRepository;

        public PhotoProductsController (IModelRepository<PhotoProduct> photoProductsRepository)
        {
            _photoProductsRepository = photoProductsRepository;
        }


        [HttpGet]
        public async Task<ActionResult<TaskResult<List<PhotoProduct>>>> GetPhotoProducts ()
        {
            try
            {
                var taskResult = await _photoProductsRepository.GetAll();
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet ("{photoProductId}")]
        public async Task<ActionResult<TaskResult<PhotoProduct>>> GetPhotoProduct (string photoProductId)
        {
            try
            {
                var taskResult = await _photoProductsRepository.Get(photoProductId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost]
        public async Task<ActionResult<TaskResult<PhotoProduct>>> PostPhotoProduct (PhotoProduct model)
        {
            try
            {
                var taskResult = await _photoProductsRepository.Create(model);
                return CreatedAtAction (nameof (GetPhotoProduct), new { photoProductId = model.PhotoProductId }, taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPut ("{photoProductId}")]
        public async Task<ActionResult<TaskResult<PhotoProduct>>> PutPhotoProduct (string photoProductId, PhotoProduct model)
        {
            try
            {
                if (photoProductId != model.PhotoProductId)
                    return BadRequest ("PhotoProductId mismatch");

                var taskResult = await _photoProductsRepository.Update(model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpDelete ("{photoProductId}")]
        public async Task<ActionResult<TaskResult<PhotoProduct>>> DeletePhotoProduct (string photoProductId)
        {
            try
            {
                var taskResult = await _photoProductsRepository.Delete(photoProductId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
