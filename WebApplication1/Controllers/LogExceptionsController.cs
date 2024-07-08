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
    public class LogExceptionsController: ControllerBase
    {
        private readonly IModelRepository <LogException> _logExceptionsRepository;

        public LogExceptionsController (IModelRepository<LogException> logExceptionsRepository)
        {
            _logExceptionsRepository = logExceptionsRepository;
        }



        [HttpGet]
        public async Task<ActionResult<TaskResult<List<LogException>>>> GetLogExceptions ()
        {
            try
            {
                var taskResult = await _logExceptionsRepository.GetAll();
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet ("{logExceptionId}")]
        public async Task<ActionResult<TaskResult<LogException>>> GetLogException (string logExceptionId)
        {
            try
            {
                var taskResult = await _logExceptionsRepository.Get(logExceptionId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost]
        public async Task<ActionResult<TaskResult<LogException>>> PostLogException (LogException model)
        {
            try
            {
                var taskResult = await _logExceptionsRepository.Create(model);
                return CreatedAtAction (nameof (GetLogException), new { logExceptionId = model.LogExceptionId }, taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPut ("{logExceptionId}")]
        public async Task<ActionResult<TaskResult<LogException>>> PutLogException (string logExceptionId, LogException model)
        {
            try
            {
                if (logExceptionId != model.LogExceptionId)
                    return BadRequest ("LogExceptionId mismatch");

                var taskResult = await _logExceptionsRepository.Update(model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpDelete ("{logExceptionId}")]
        public async Task<ActionResult<TaskResult<LogException>>> DeleteLogException (string logExceptionId)
        {
            try
            {
                var taskResult = await _logExceptionsRepository.Delete(logExceptionId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
