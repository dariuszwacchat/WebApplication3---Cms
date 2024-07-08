using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Services;
using Domain.ViewModels;
using Domain.Models;

namespace WebApplication1.Controllers
{
    [Authorize (Roles = "Administrator")]
    [Route ("api/[controller]")] 
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController (AccountService accountService)
        {
            _accountService = accountService;
        }
         
        [HttpGet ("getUsers")]
        public async Task<ActionResult<TaskResult<List<ApplicationUser>>>> GetUseres ()
        {
            try
            {
                var taskResult = await _accountService.GetUsers ();
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet ("getUserById/{userId}")]
        public async Task<ActionResult<TaskResult<ApplicationUser>>> GetUserById (string userId)
        {
            try
            {
                var taskResult = await _accountService.GetUserById (userId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet ("getUserByEmail/{email}")]
        public async Task<ActionResult<TaskResult<ApplicationUser>>> GetUserByEmail (string email)
        {
            try
            {
                var taskResult = await _accountService.GetUserByEmail (email);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }



        [AllowAnonymous]
        [HttpPost ("register")]
        public async Task<ActionResult<TaskResult<RegisterViewModel>>> Register (RegisterViewModel model)
        {
            try
            {
                var taskResult = await _accountService.Register (model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            { 
                return Unauthorized ();
            }
        }


        [AllowAnonymous]
        [HttpPost ("login")]
        public async Task<ActionResult<TaskResult<LoginViewModel>>> Login (LoginViewModel model)
        {
            try
            {
                var taskResult = await _accountService.Login (model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return Unauthorized ();
            }
        }




        [AllowAnonymous]
        [HttpPost ("logout")]
        public async Task<IActionResult> Logout ()
        {
            await _accountService.Logout ();
            return Ok ();
        }
         
         
        
        
        [HttpPost ("changeEmail")]
        public async Task<ActionResult<TaskResult<ChangeEmailViewModel>>> ChangeEmail (ChangeEmailViewModel model)
        {
            try
            {
                var taskResult = await _accountService.ChangeEmail (model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost ("changePassword")]
        public async Task<ActionResult<TaskResult<ChangePasswordViewModel>>> ChangeEmail (ChangePasswordViewModel model)
        {
            try
            {
                var taskResult = await _accountService.ChangePassword (model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost ("updateAccount")]
        public async Task<ActionResult<TaskResult<ApplicationUser>>> UpdateAccount (ApplicationUser model)
        {
            try
            {
                var taskResult = await _accountService.UpdateAccount (model);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        } 


        [HttpDelete ("deleteAccountByUserId/{userId}")]
        public async Task<ActionResult<bool>> DeleteAccountByUserId (string userId)
        {
            try
            {
                var taskResult = await _accountService.DeleteAccountByUserId (userId);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpDelete ("deleteAccountByEmail/{email}")]
        public async Task<ActionResult<TaskResult<bool>>> DeleteAccountByEmail (string email)
        {
            try
            {
                var taskResult = await _accountService.DeleteAccountByEmail (email);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet ("getUserRoles/{email}")]
        public async Task<ActionResult<TaskResult<List<string>>>> GetUserRoles (string email)
        {
            try
            {
                var taskResult = await _accountService.GetUserRoles (email);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet ("userInRole/{email}/{roleName}")]
        public async Task<ActionResult<TaskResult<bool>>> UserInRole (string email, string roleName)
        {
            try
            {
                var taskResult = await _accountService.UserInRole (email, roleName);
                return Ok (taskResult);
            }
            catch (Exception ex)
            {
                return StatusCode (500, $"Internal server error: {ex.Message}");
            }
        }





    }
}
