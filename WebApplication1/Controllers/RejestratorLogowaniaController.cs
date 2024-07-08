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
    public class RejestratorLogowaniaController: ControllerBase
    {
        private readonly IModelRepository <RejestratorLogowania> _rejestratorLogowania;

        public RejestratorLogowaniaController (IModelRepository<RejestratorLogowania> rejestratorLogowania)
        {
            _rejestratorLogowania = rejestratorLogowania;
        }



        [HttpGet]
        public async Task<ActionResult<TaskResult<List<RejestratorLogowania>>>> GetRejestratorLogowanias ()
        {
            try
            {
                var taskResult = await _rejestratorLogowania.GetAll();
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet ("{rejestratorLogowaniaId}")]
        public async Task<ActionResult<TaskResult<RejestratorLogowania>>> GetRejestratorLogowania (string rejestratorLogowaniaId)
        {
            try
            {
                var taskResult = await _rejestratorLogowania.Get(rejestratorLogowaniaId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost]
        public async Task<ActionResult<TaskResult<RejestratorLogowania>>> PostRejestratorLogowania (RejestratorLogowania model)
        {
            try
            {
                var taskResult = await _rejestratorLogowania.Create(model);
                return CreatedAtAction (nameof (GetRejestratorLogowania), new { rejestratorLogowaniaId = model.RejestratorLogowaniaId }, taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPut ("{rejestratorLogowaniaId}")]
        public async Task<ActionResult<TaskResult<RejestratorLogowania>>> PutRejestratorLogowania (string rejestratorLogowaniaId, RejestratorLogowania model)
        {
            try
            {
                if (rejestratorLogowaniaId != model.RejestratorLogowaniaId)
                    return BadRequest ("RejestratorLogowaniaId mismatch");

                var taskResult = await _rejestratorLogowania.Update(model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpDelete ("{rejestratorLogowaniaId}")]
        public async Task<ActionResult<TaskResult<RejestratorLogowania>>> DeleteMovie (string rejestratorLogowaniaId)
        {
            try
            {
                var taskResult = await _rejestratorLogowania.Delete(rejestratorLogowaniaId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
