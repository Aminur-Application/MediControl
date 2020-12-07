using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using MediControl.Models;
using MediControl.Entities;
using MediControl.Helpers;
using MediControl.BindingModels;
using System.Threading.Tasks;

namespace MediControl.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(UserLogin model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        //private readonly AppSettings _appSettings;

        private UserManager<User> _userManager;
        private RoleManager<IdentityRole<Guid>> _roleManager;
        private SignInManager<User> _signInManager;


        public UserService(ApplicationDbContext context,UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, SignInManager<User> signInManager)
        {
            this._context = context;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
        }

        public async Task<AuthenticateResponse> Authenticate(UserLogin model, string ipAddress)
        {
            //var user =  _.Users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);
            User user = await _userManager.FindByNameAsync(model.Username);

            if(user == null){
                return null;
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            // return null if user not found
            
             if (signInResult.Succeeded)
            {
                var jwtToken = generateJwtToken(user);
                var refreshToken = generateRefreshToken(ipAddress);

            // save refresh token
                user.RefreshTokens.Add(refreshToken);
                _context.Update(user);
                _context.SaveChanges();
                return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
            }
            else{

             return null;
            }
            // authentication successful so generate jwt and refresh tokens
        
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            
            // return null if no user found with token
            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _context.Update(user);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            
            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtModelConstants.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = JwtModelConstants.Issuer,
                Audience = JwtModelConstants.Audience,
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using(var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}