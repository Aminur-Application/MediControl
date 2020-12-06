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
using MediControl.Entities;
using MediControl.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using MediControl.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace MediControl.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private RoleManager<IdentityRole<Guid>> roleManager;
        private SignInManager<User> signInManager;

        private IUserService _userService;

        public AccountController(IUserService userService, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, SignInManager<User> signInManager)
        {
            this._userService = userService;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;

            if (!roleManager.Roles.Any())
            {
                var result = roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin" }).Result;
                result = roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Employee" }).Result;
                result = roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "SuperAdmin" }).Result;

                var user = new User { UserName = "SuperAdmin", Email = "SuperAdmin123@asd.co.uk", ClearanceNumber = 5, FirstName = "Super", LastName = "Admin" };
                result = userManager.CreateAsync(user, "SuperAdmin-271485").Result;
                result = userManager.AddToRoleAsync(user, "SuperAdmin").Result;


                user = new User { UserName = "Admin", Email = "Admin123@asd.co.uk", ClearanceNumber = 4, FirstName = "Admin", LastName = "Admin" };
                result = userManager.CreateAsync(user, "Admin-271485").Result;
                result = userManager.AddToRoleAsync(user, "Admin").Result;

                user = new User { UserName = "user", Email = "user@asd.co.uk", ClearanceNumber = 1, FirstName = "User", LastName = "User" };
                result = userManager.CreateAsync(user, "User-271485").Result;
                result = userManager.AddToRoleAsync(user, "Employee").Result;
            }
        }

        [HttpGet("/api/login")]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost("/api/login")]
        public async Task<IActionResult> Login([FromBody] UserLogin model)
        {

            var response = await _userService.Authenticate(model, ipAddress());

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            setTokenCookie(response.RefreshToken);

            return Ok(response);
            //  User user = await userManager.FindByNameAsync(model.Username);
            // var signInResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            // if (signInResult.Succeeded)
            // {
            //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtModelConstants.Key));
            //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //     var claims = new[]
            //     {
            //         new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, model.Username),
            //         new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //         new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName,model.Username)

            //     };

            //     var token = new JwtSecurityToken(
            //         JwtModelConstants.Issuer,
            //         JwtModelConstants.Audience,
            //         claims,
            //         expires: DateTime.UtcNow.AddMinutes(5),
            //         signingCredentials: creds

            //         );

            //     var result = new
            //     {
            //         token = new JwtSecurityTokenHandler().WriteToken(token),
            //         expiration = token.ValidTo
            //     };

            //     return Ok(result);


        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshToken(refreshToken, ipAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            setTokenCookie(response.RefreshToken);

            return Ok(response);
        }


        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }


        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        [HttpPost("/api/getusers")]
        public ActionResult getUsers([FromBody] PublicUserInfo info)
        {



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
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: creds
                    );

                var result = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                };

                return Created("", result);


            }
            else
            {
                return BadRequest();
            }

        }
    }
}