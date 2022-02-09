﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Claim.Data.Enties;
using Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.BindingModel;
using Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Train.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signManager;
        private readonly JWTConfig _jwtConfig;

        public UserController(ILogger<UserController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signManager,IOptions<JWTConfig> jwtConfig)
        {
            _userManager = userManager;
            _signManager = signManager;
            _logger = logger;
            _jwtConfig = jwtConfig.Value;
        }

    //Register
    [HttpPost("RegisterUser")]
    public async Task<object> RegisterUser([FromBody] AddUpdateRegisterUserBindingModel model)
    {
        try
        {
        var user = new AppUser()
        {
            FullName = model.FullName,
            Email = model.Email,
            UserName = model.Email,
            DataCreated = DateTime.UtcNow,
            DataModified = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(user,model.Password);
        if(result.Succeeded)
        {
            return await Task.FromResult("User has been Registered");
        }
        return await Task.FromResult(string.Join(",",result.Errors.Select(x=>x.Description).ToArray()));
        }catch(Exception ex){
            return await Task.FromResult(ex.Message);
        }
    }


    //Get User
    
    [HttpGet("GetAllUser")]
    [Authorize]
    public async Task<object> GetAllUser()
    {
        try
        {
            var user = _userManager.Users.Select(x=>new UserDTO(x.FullName,x.Email,x.UserName,x.DataCreated));
            return await Task.FromResult(user);
        }catch(Exception ex)
        {
            return await Task.FromResult(ex.Message);
        }
    }


    //Login
    [HttpPost("Login")]
    public async Task<object> Login([FromBody] LoginBindingModel model)
    {
        try
        {
            if(ModelState.IsValid)
            {
                
            var result = await _signManager.PasswordSignInAsync(model.Email, model.Password, false,false);
            if(result.Succeeded)
            {
                var appUser = await _userManager.FindByEmailAsync(model.Email);
                var user = new UserDTO(appUser.FullName,appUser.Email,appUser.UserName,appUser.DataCreated);
                user.Token = GenerateToken(appUser);
                return await Task.FromResult(user);
            }
            }
            return await Task.FromResult("Invalid Email or Password");
        }catch(Exception ex)
        {
            return await Task.FromResult(ex.Message);
        }
    }


    //Generate Token
    private string GenerateToken(AppUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);
        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new System.Security.Claims.ClaimsIdentity(new[]{
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.NameId,user.Id),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email,user.Email),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            }),
            Expires = DateTime.UtcNow.AddHours(12),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience
        };
        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        return jwtTokenHandler.WriteToken(token);
    }
        
    }
}