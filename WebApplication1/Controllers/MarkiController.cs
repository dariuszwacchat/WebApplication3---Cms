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
    public class MarkiController: ControllerBase
    {
        private readonly IModelRepository <Marka> _markiRepository;

        public MarkiController (IModelRepository<Marka> markiRepository)
        {
            _markiRepository = markiRepository;
        }


        [HttpGet]
        public async Task<ActionResult<TaskResult<List<Marka>>>> GetMarki ()
        {
            try
            {
                var taskResult = await _markiRepository.GetAll();
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet ("{markaId}")]
        public async Task<ActionResult<TaskResult<Marka>>> GetMarka (string markaId)
        {
            try
            {
                var taskResult = await _markiRepository.Get(markaId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost]
        public async Task<ActionResult<TaskResult<Marka>>> PostMarka (Marka model)
        {
            try
            {
                var taskResult = await _markiRepository.Create(model);
                return CreatedAtAction (nameof (GetMarka), new { markaId = model.MarkaId }, taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPut ("{markaId}")]
        public async Task<ActionResult<TaskResult<Marka>>> PutMarka (string markaId, Marka model)
        {
            try
            {
                if (markaId != model.MarkaId)
                    return BadRequest ("Marka mismatch");

                var taskResult = await _markiRepository.Update(model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpDelete ("{markaId}")]
        public async Task<ActionResult<TaskResult<Marka>>> DeleteMarka (string markaId)
        {
            try
            {
                var taskResult = await _markiRepository.Delete(markaId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
