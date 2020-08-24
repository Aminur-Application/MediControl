using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediControl.BindingModels;
using MediControl.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace MediControl.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private RoleManager<IdentityRole<Guid>> roleManager;
        private SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;

            if (!roleManager.Roles.Any())
            {
                var result = roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin", }).Result;
                result = roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Employee" }).Result;
                result = roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "SuperAdmin" }).Result;

                var user = new User { UserName = "SuperAdmin", Email = "SuperAdmin123@asd.co.uk", ClearanceNumber = 5 };
                result = userManager.CreateAsync(user, "SuperAdmin-271485").Result;
                result = userManager.AddToRoleAsync(user, "SuperAdmin").Result;


                user = new User { UserName = "Admin", Email = "Admin123@asd.co.uk", ClearanceNumber = 4 };
                result = userManager.CreateAsync(user, "Admin-271485").Result;
                result = userManager.AddToRoleAsync(user, "Admin").Result;

                user = new User { UserName = "user", Email = "user@asd.co.uk", ClearanceNumber = 1};
                result = userManager.CreateAsync(user, "User-271485").Result;
                result = userManager.AddToRoleAsync(user, "Employee").Result;
            }
        }

        [HttpGet("/api/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/api/login")]
        public async Task<IActionResult> Login(UserLogin model)
        {
             User user = await userManager.FindByNameAsync(model.Username);
            var signInResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (signInResult.Succeeded)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtModelConstants.Key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, model.Username),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName,model.Username)
                };

                var token = new JwtSecurityToken(
                    JwtModelConstants.Issuer,
                    JwtModelConstants.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds
                    );

                var result = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                };

                return Created("", result);


            } else
            {
                return BadRequest();
            }
            /* // //User user;
            // //Microsoft.AspNetCore.Identity.SignInResult result;

            // User signinuser = await userManager.FindByEmailAsync(model.Username);

            // Microsoft.AspNetCore.Identity.SignInResult signin = signInManager.PasswordSignInAsync(signinuser.UserName, model.Password, false, false).Result;

            // if (signin.Succeeded)
            // {

            //     PublicUserInfo info = new PublicUserInfo
            //     {

            //         UserClearanceNumber = signinuser.ClearanceNumber,
            //         role = userManager.GetRolesAsync(signinuser).Result,
            //         Username = signinuser.UserName


            //     };
            //     return Ok(JsonConvert.SerializeObject(info));
            // }

            // return Unauthorized();



            //if (ModelState.IsValid && (user = userManager.FindByEmailAsync(model.Username)) != null
            //    && (result = signInManager.PasswordSignInAsync(user, model.Password, false, false)).Succeeded)
            //{
            //    UserLogin info = new UserLogin
            //    {
            //        Username = model.Username,
            //        Password = model.Password
            //    };

            //    return info;
            //} */


        }

        [HttpPost("/api/getusers")]
        public ActionResult getUsers([FromBody]PublicUserInfo info){


            
            var users = userManager.Users.Select(c => c.ClearanceNumber < info.UserClearanceNumber);
            return Ok(JsonConvert.SerializeObject(users));
        }

        // [HttpPost("/api/getusers")]
        // public async Task<IActionResult> SignOut()
        // {
        //     await signInManager.SignOutAsync();
        //     return RedirectToAction("Login");
        // }

        [HttpPost("/api/createtoken")]
        public async Task<IActionResult> CreateToken([FromBody] JwtTokenViewModel model)
        {

            User user = await userManager.FindByNameAsync(model.Username);
            var signInResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (signInResult.Succeeded)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtModelConstants.Key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, model.Username),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName,model.Username)
                };

                var token = new JwtSecurityToken(
                    JwtModelConstants.Issuer,
                    JwtModelConstants.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds
                    );

                var result = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                };

                return Created("", result);


            } else
            {
                return BadRequest();
            }
            
        }
    }
}