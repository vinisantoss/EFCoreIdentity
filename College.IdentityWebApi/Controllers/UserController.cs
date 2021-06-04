using AutoMapper;
using College.IdentityWebApi.Domain;
using College.IdentityWebApi.Domain.Services.Interface;
using College.IdentityWebApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College.IdentityWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwtGenerator;

        public UserController(IConfiguration configuration,
                              UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IMapper mapper,
                              IJwtGenerator jwtGenerator)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _jwtGenerator = jwtGenerator;
        }


        [HttpGet]
        [AllowAnonymous]
        public UserModel Get()
        {
            return new UserModel();
        }


        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginModel userLoginModel)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userLoginModel.UserName);

                if(user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, userLoginModel.Password, false);

                    if (result.Succeeded)
                    {
                        var appUser = await _userManager.Users.FirstOrDefaultAsync(user => user.NormalizedUserName == userLoginModel.UserName.ToUpper());
                        var userToApplication = _mapper.Map<UserModel>(appUser);

                        return Ok( 
                            new { token = await _jwtGenerator.GenerateJwtToken(user), 
                                  user = userToApplication
                            });
                    }
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message} ao registrar usuário!");
            }
        }


        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserModel userModel)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userModel.UserName);
                if (user == null)
                {
                    user = new User()
                    {
                        UserName = userModel.UserName,
                        Email = userModel.Email,
                        FullName = userModel.FullName
                    };

                    var result = await _userManager.CreateAsync(user, userModel.Password);

                    if (result.Succeeded)
                    {

                        var appUser = await _userManager.Users.FirstOrDefaultAsync(user => user.NormalizedUserName == userModel.UserName.ToUpper());
                        var token = await _jwtGenerator.GenerateJwtToken(user);

                        //var confirmationEmail = Url.Action("ConfirmEmailAdress", "Register",
                        //            new { token = token, email = user.Email }, Request.Scheme);

                        //System.IO.File.WriteAllText("ConfirmationEmail.txt", confirmationEmail);

                        return Ok(token);
                    }

                    
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message} ao registrar usuário!");
            }
            
        }


        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }


        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
