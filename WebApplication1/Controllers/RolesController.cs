using Application.Services.Abs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize (Roles = "Administrator")]
    [Route ("api/[controller]")]
    [ApiController]
    public class RolesController: ControllerBase
    {
        private readonly IRolesService <ApplicationRole> _rolesService;

        public RolesController (IRolesService<ApplicationRole> rolesService)
        {
            _rolesService = rolesService;
        }



        [HttpGet]
        public async Task<ActionResult<TaskResult<List<ApplicationRole>>>> GetRoles ()
        {
            try
            {
                var taskResult = await _rolesService.GetAll();
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet ("{roleId}")]
        public async Task<ActionResult<TaskResult<ApplicationRole>>> GetRole (string roleId)
        {
            try
            {
                var taskResult = await _rolesService.Get(roleId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost]
        public async Task<ActionResult<TaskResult<ApplicationRole>>> PostRole (ApplicationRole model)
        {
            try
            {
                var taskResult = await _rolesService.Create(model);
                return CreatedAtAction (nameof (GetRole), new { roleId = model.Id }, taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPut ("{roleId}")]
        public async Task<ActionResult<TaskResult<ApplicationRole>>> PutRole (string roleId, ApplicationRole model)
        {
            try
            {
                if (roleId != model.Id)
                    return BadRequest ("RoleId mismatch");

                var taskResult = await _rolesService.Update(model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpDelete ("{roleId}")]
        public async Task<ActionResult<TaskResult<ApplicationRole>>> DeleteRole (string roleId)
        {
            try
            {
                var taskResult = await _rolesService.Delete(roleId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
