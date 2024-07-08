using Data;
using Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;


namespace Application.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;  
        private readonly IConfiguration _configuration;

        public AccountService (ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<TaskResult<List<ApplicationUser>>> GetUsers ()
        {
            var taskResult = new TaskResult <List<ApplicationUser>> () { Success = true, Model = new List<ApplicationUser> (), Message = "" };

            try
            {
                var users = await _context.Users.ToListAsync ();

                if (users == null)
                {
                    taskResult.Success = false;
                    taskResult.Message = "Movies was null";
                }
                else
                {
                    taskResult.Success = true;
                    taskResult.Model = users;
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }
            return taskResult;
        }



        public async Task<TaskResult<ApplicationUser>> GetUserById (string userId)
        {
            var taskResult = new TaskResult <ApplicationUser> () { Success = true, Model = new ApplicationUser (), Message = "" };

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync (f => f.Id == userId);
                if (user != null)
                {
                    taskResult.Success = true;
                    taskResult.Model = user;
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "Wskazany adres email nie istnieje";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }
            return taskResult;
        }



        public async Task<TaskResult <ApplicationUser>> GetUserByEmail (string email)
        {
            var taskResult = new TaskResult <ApplicationUser> () { Success = true, Model = new ApplicationUser (), Message = "" };

            try
            { 
                var user = await _context.Users.FirstOrDefaultAsync (f => f.Email == email); 
                if (user != null)
                {
                    taskResult.Success = true;
                    taskResult.Model = user;
                } 
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "Wskazany adres email nie istnieje";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }
            return taskResult;
        }



        public async Task<TaskResult<RegisterViewModel>> Register (RegisterViewModel model)
        {
            var taskResult = new TaskResult <RegisterViewModel> () { Success = true, Model = new RegisterViewModel (), Message = "" };

            try
            {
                var findUser = await _userManager.FindByEmailAsync(model.Email);  
                if (findUser == null)
                {
                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString (),
                        Email = model.Email,
                        UserName = model.Email,

                        Imie = model.Imie,
                        Nazwisko = model.Nazwisko,
                        Ulica = model.Ulica,
                        NumerUlicy = model.NumerUlicy,
                        Miejscowosc = model.Miejscowosc,
                        Kraj = model.Kraj,
                        KodPocztowy = model.KodPocztowy,
                        DataUrodzenia = model.DataUrodzenia,
                        Telefon = model.Telefon, 
                        DataDodania = DateTime.Now.ToString (),

                        ConcurrencyStamp = Guid.NewGuid ().ToString (),
                        SecurityStamp = Guid.NewGuid ().ToString (),
                        NormalizedEmail = model.Email.ToUpper(),
                        NormalizedUserName = model.Email.ToUpper(),
                        EmailConfirmed = false,
                    };
                     

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync (user, isPersistent: false);

                        string token = GenerateJwtToken(user, model.RoleName);

                        // dodanie użytkownika do roli 
                        /*foreach (var role in model.Roles)
                        {
                            await _userManager.AddToRoleAsync (user, role);
                        }*/
                        await _userManager.AddToRoleAsync (user, model.RoleName);
                         
                        var role = await _context.Roles.FirstOrDefaultAsync (f=> f.Name == model.RoleName);
                        if (role != null)
                        {
                            user.RoleId = role.Id;
                            await _userManager.UpdateAsync (user);
                        } 

                        taskResult.Success = true;
                        taskResult.Model = new RegisterViewModel ()
                        {
                            Email = user.Email, 
                            Token = token,
                            //Roles = model.Roles,
                            RoleName = model.RoleName,
                        };
                        taskResult.Message = "Zarejestrowano";
                    }
                    else
                    { 
                        taskResult.Success = false;
                        taskResult.Message = "Nie można zarejestrować użytkownika";
                    }
                }
                else
                { 
                    taskResult.Success = false;
                    taskResult.Message = "Wskazany adres email już istnieje";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            } 
             
            return taskResult;
        }


        /*public async Task<TaskResult<LoginViewModel>> Login (LoginViewModel model)
        {
            var taskResult = new TaskResult <LoginViewModel> () { Success = true, Model = new LoginViewModel (), Message = "" };

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {

                        // pobranie roli użytkownika
                        var userRoles = await _userManager.GetRolesAsync (user);
                        string firstRole = (userRoles != null && userRoles.Count > 0) ? userRoles[0] : "";

                        taskResult.Success = true;
                        taskResult.Model = new LoginViewModel ()
                        {
                            Email = user.Email,
                            Password = user.PasswordHash,
                            Token = GenerateJwtToken (user, firstRole),
                            Role = firstRole
                        };
                    }
                    else
                    {
                        taskResult.Success = false;
                        taskResult.Message = "Błędny login lub hasło";
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "Użytkownik nie istnieje";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }*/



        public async Task<TaskResult<LoginViewModel>> Login (LoginViewModel model)
        {
            var taskResult = new TaskResult <LoginViewModel> () { Success = true, Model = new LoginViewModel (), Message = "" };

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // sprawdź uprawnienia

                    var userRoles = await _userManager.GetRolesAsync (user);
                    string firstRole = (userRoles != null && userRoles.Count > 0) ? userRoles[0] : "";

                    // zalogować się może tylko i wyłącznie administrator
                    if (firstRole == "Administrator")
                    { 
                        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: true);
                        if (result.Succeeded)
                        {


                            taskResult.Success = true;
                            taskResult.Model = new LoginViewModel ()
                            {
                                Email = user.Email,
                                Password = user.PasswordHash,
                                Token = GenerateJwtToken (user, firstRole),
                                Role = firstRole
                            };
                        }
                        else
                        {
                            taskResult.Success = false;
                            taskResult.Message = "Błędny login lub hasło";
                        }
                    }
                    else
                    {
                        taskResult.Success = false;
                        taskResult.Message = "Konto dostępne wyłącznie dla administartora systemu";
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "Użytkownik nie istnieje";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }



        public async Task Logout ()
        {
            await _signInManager.SignOutAsync ();
        }







        public async Task<TaskResult<ChangeEmailViewModel>> ChangeEmail (ChangeEmailViewModel model)
        {
            var taskResult = new TaskResult <ChangeEmailViewModel> () { Success = true, Model = new ChangeEmailViewModel (), Message = "" };

            try
            {
                // Sprawdza czy pola nie są puste
                if (model != null)
                {
                    // wyszukuja użytkownika na podstawie emaila
                    if ((await _context.Users.FirstOrDefaultAsync (f => f.Email == model.NewEmail)) == null)
                    {
                        ApplicationUser user = await _userManager.FindByNameAsync (model.UserName);
                        if (user != null)
                        {
                            string token = await _userManager.GenerateChangeEmailTokenAsync (user, model.NewEmail);
                            var result = await _userManager.ChangeEmailAsync (user, model.NewEmail, token);
                            if (result.Succeeded)
                            {  
                                // zaktualizowanie nazwy użytkownika 
                                user.UserName = model.NewEmail;
                                await _userManager.UpdateAsync (user);
                                //await _signInManager.SignOutAsync ();
                                taskResult.Success = false;
                            }
                            else
                            {
                                taskResult.Success = false;
                                taskResult.Message = "Email nie został zmieniony";
                            }
                        }
                    }
                    else
                    {
                        taskResult.Success = false;
                        taskResult.Message = "Użytkownik o takim adresie email już istnieje";
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "Model was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }
            return taskResult;
        }



        public async Task<TaskResult<ChangePasswordViewModel>> ChangePassword (ChangePasswordViewModel model)
        {
            var taskResult = new TaskResult <ChangePasswordViewModel> () { Success = true, Model = new ChangePasswordViewModel (), Message = "" };

            try
            {
                if (model != null)
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync (model.Email);
                    if (user != null)
                    {
                        IdentityResult result = await _userManager.ChangePasswordAsync (user, model.OldPassword, model.NewPassword);
                        if (result.Succeeded)
                        {
                            taskResult.Success = true;

                            // wylogowanie
                            //await _signInManager.SignOutAsync ();
                        }
                        else
                        {
                            taskResult.Success = false;
                            taskResult.Message = "Błędne hasło";
                        }
                    }
                    else
                    {
                        taskResult.Success = false;
                        taskResult.Message = "Wskazany użytkownik nie istnieje";
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "Model was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }




        public async Task<TaskResult<ApplicationUser>> UpdateAccount (ApplicationUser model)
        {
            var taskResult = new TaskResult <ApplicationUser> () { Success = true, Model = new ApplicationUser (), Message = "" };

            try
            {
                if (model != null)
                {
                    ApplicationUser user = await _context.Users.FirstOrDefaultAsync (f=> f.Id == model.Id);
                    if (user != null)
                    {
                        /*user.Email = model.Email;
                        user.UserName = model.Email;*/

                        user.Imie = model.Imie;
                        user.Nazwisko = model.Nazwisko;
                        user.Ulica = model.Ulica;
                        user.NumerUlicy = model.NumerUlicy;
                        user.Miejscowosc = model.Miejscowosc;
                        user.KodPocztowy = model.KodPocztowy;
                        user.Kraj = model.Kraj;
                        user.DataUrodzenia = model.DataUrodzenia;
                        user.Telefon = model.Telefon;


                        var userRoles = await _userManager.GetRolesAsync (user);
                        foreach (var role in userRoles)
                        {
                            await _userManager.RemoveFromRoleAsync (user, role);
                        }
                        
                        user.RoleId = model.RoleId;


                        var result = await _userManager.UpdateAsync (user);
                        if (result.Succeeded)
                        {  
                            // dodanie zdjęcia
                            //await CreateNewPhoto (model.Files, user.Id);

                            /*
                                                        // Usunięcie ze wszystkich rół
                                                        foreach (var role in await _context.Roles.ToListAsync ())
                                                            await _userManager.RemoveFromRoleAsync (user, role.Name);


                                                        // Przypisanie nowych ról
                                                        foreach (var selectedRole in model.SelectedRoles)
                                                        {
                                                            await _userManager.AddToRoleAsync (user, selectedRole);
                                                            await _userManager.AddClaimAsync (user, new Claim (ClaimTypes.Role, selectedRole));
                                                        }
                            */


                            taskResult.Success = true;
                            taskResult.Model = user;
                            taskResult.Message = "Dane zostały zaktualizowane poprawnie";
                        }
                        else
                        {
                            taskResult.Success = false;
                            taskResult.Message = "Dane nie zostały zaktualizowane";
                        }
                    }
                    else
                    {
                        taskResult.Success = false;
                        taskResult.Message = "User was null";
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "Model was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }









        public async Task<TaskResult<bool>> DeleteAccountByUserId (string userId)
        {
            var taskResult = new TaskResult <bool> () { Success = true, Model = true, Message = "" };

            try
            {
                if (!string.IsNullOrEmpty (userId))
                {
                    ApplicationUser user = await _userManager.FindByIdAsync (userId);
                    if (user != null)
                    {

/*
                        // usunięcie zdjęć
                        var photosUser = await _context.PhotosUser.Where (w=> w.UserId == user.Id).ToListAsync ();
                        foreach (var photoUser in photosUser)
                            _context.PhotosUser.Remove (photoUser);
*/

                        var result =  await _userManager.DeleteAsync(user);
                        if (result.Succeeded)
                        {
                            // Wylogowanie użytkownika
                            //await _signInManager.SignOutAsync();
                            taskResult.Success = true;
                        }
                    }
                    else
                    {
                        taskResult.Success = false;
                        taskResult.Message = "User was null";
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "Model was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }





        public async Task<TaskResult<bool>> DeleteAccountByEmail (string email)
        {
            var taskResult = new TaskResult <bool> () { Success = true, Model = true, Message = "" };

            try
            {
                if (!string.IsNullOrEmpty (email))
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync (email);
                    if (user != null)
                    {
                        /*
                        // usunięcie zdjęć
                        var photosUser = await _context.PhotosUser.Where (w=> w.UserId == user.Id).ToListAsync ();
                        foreach (var photoUser in photosUser)
                            _context.PhotosUser.Remove (photoUser);
                        */

                        var result =  await _userManager.DeleteAsync(user);
                        if (result.Succeeded)
                        {
                            // Wylogowanie użytkownika
                            //await _signInManager.SignOutAsync();
                            taskResult.Success = false;
                        }
                    }
                    else
                    {
                        taskResult.Success = false;
                        taskResult.Message = "User was null";
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "Model was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }






        /// <summary>
        /// Usuwa użytkownika z roli
        /// </summary>
        public async Task<TaskResult <bool>> RemoveFromRole (string userName, string roleName)
        {
            var taskResult = new TaskResult <bool> () { Success = true, Model = true, Message = "" };

            try
            { 
                ApplicationUser user = await _userManager.FindByNameAsync (userName);
                if (user != null)
                {
                    IdentityResult result =  await _userManager.RemoveFromRoleAsync (user, roleName);
                    if (result.Succeeded)
                    { 
                        taskResult.Success = true;
                    }
                    else
                    {
                        taskResult.Success = false;
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "User was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }
            return taskResult;
        }


        /// <summary>
        /// Dodaje użytkownika do roli
        /// </summary>
        public async Task<TaskResult <bool>> AddToRole (string email, string roleName)
        {
            var taskResult = new TaskResult <bool> () { Success = true, Model = true, Message = "" };

            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync (email);
                if (user != null)
                {
                    var result =  await _userManager.AddToRoleAsync (user, roleName);
                    if (result.Succeeded)
                    {
                        taskResult.Success = true;
                    }
                    else
                    {
                        taskResult.Success = false;
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "User was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }

        


        public async Task<TaskResult <bool>> AddClaim (string email, string roleName)
        {
            var taskResult = new TaskResult <bool> () { Success = true, Model = true, Message = "" };

            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync (email);
                if (user != null)
                {
                    var result =  await _userManager.AddClaimAsync (user, new Claim (ClaimTypes.Role, roleName));
                    if (result.Succeeded)
                    {
                        taskResult.Success = true;
                    }
                    else
                    {
                        taskResult.Success = false;
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "User was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }



        /// <summary>
        /// Pobiera wszystkie role danego użytkownika
        /// </summary>
        public async Task<TaskResult <List<string>>> GetUserRoles (string email)
        {
            var taskResult = new TaskResult <List<string>> () { Success = true, Model = new List<string> (), Message = "" };

            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync (email);
                if (user != null)
                {
                    var userRoles = (await _userManager.GetRolesAsync (user)).ToList ();

                    taskResult.Success = true;
                    taskResult.Model = userRoles;
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "User was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }
            return taskResult;
        }




        /// <summary>
        /// Pobiera wszystkich użytkowników będących w danej roli
        /// </summary>
        public async Task<TaskResult <List<ApplicationUser>>> GetUsersInRole (string roleName)
        {
            var taskResult = new TaskResult <List <ApplicationUser>> () { Success = true, Model = new List <ApplicationUser> (), Message = "" };

            try
            {
                var usersInRole = (await _userManager.GetUsersInRoleAsync (roleName)).ToList ();
                if (usersInRole != null)
                {
                    taskResult.Success = true;
                    taskResult.Model = usersInRole;
                }
                else
                {
                    taskResult.Success = false;
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }



        /// <summary>
        /// Sprawdza czy użytkownik jest w danej roli
        /// </summary>
        public async Task<TaskResult<bool>> UserInRole (string email, string roleName)
        {
            var taskResult = new TaskResult <bool> () { Success = true, Model = true, Message = "" };
          
            try
            {
                var user = await _userManager.FindByEmailAsync (email);
                if (user != null)
                {
                    var userInRole = await _userManager.IsInRoleAsync (user, roleName);
                    if (userInRole)
                    {
                        taskResult.Success = true;  
                        taskResult.Model = true;
                    } 
                    else
                    {
                        taskResult.Success = false;
                    }
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "User was null";
                }

            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }



        /// <summary>
        /// Sprawdza czy zalogowany user jest administratorem, jeśli tak to przekierowuje go do panelu administratora
        /// </summary>
        public async Task<TaskResult<bool>> LoggedUserIsAdmin (string email)
        {
            var taskResult = new TaskResult <bool> () { Success = true, Model = true, Message = "" };

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync (f=> f.Email == email);
                if (user != null)
                {
                    taskResult.Success = await _userManager.IsInRoleAsync (user, "Administrator");
                }
                else
                {
                    taskResult.Success = false;
                    taskResult.Message = "User was null";
                }
            }
            catch (Exception ex)
            {
                taskResult.Success = false;
                taskResult.Message = ex.Message;
            }

            return taskResult;
        }




        private async Task CreateNewPhoto (List<IFormFile> files, string userId)
        {
            /*try
            {
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            byte [] photoData;
                            using (var stream = new MemoryStream ())
                            {
                                file.CopyTo (stream);
                                photoData = stream.ToArray ();

                                PhotoUser photoUser = new PhotoUser ()
                                {
                                    PhotoUserId = Guid.NewGuid ().ToString (),
                                    PhotoData = photoData,
                                    UserId = userId
                                };
                                _context.PhotosUser.Add (photoUser);
                                await _context.SaveChangesAsync ();
                            }
                        }
                    }
                }
            }
            catch { }*/
        }





        private string GenerateJwtToken (ApplicationUser user, string role)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler ().WriteToken (token);
        }

    }
}
