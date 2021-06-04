using College.Identity.Identity;
using College.Identity.Models;
using College.IdentityEF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace College.Identity.Controllers
{

    public class RegisterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public RegisterController(ILogger<HomeController> logger, UserManager<User> userManager)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel register)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(register.UserName);
                if(user == null)
                {
                    user = new User()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = register.UserName,
                        Email = register.Email
                    };

                    var result = await _userManager.CreateAsync(user, register.Password);

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationEmail = Url.Action("ConfirmEmailAdress", "Register",
                                    new { token = token, email = user.Email }, Request.Scheme);

                        System.IO.File.WriteAllText("ConfirmationEmail.txt", confirmationEmail);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                    return View();

                }
            }

            return RedirectToAction("Success");
        }



        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPassword.Email);

                if(user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetURL = Url.Action("ResetPassword", "Register",
                                                new { token = token, email = forgotPassword.Email }, Request.Scheme);

                    System.IO.File.WriteAllText("resetLink.txt", resetURL);

                    return View("Success");
                }

                else
                {
                    return RedirectToAction("Error");
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            return View(new ResetPasswordModel { Token = token, Email = email});
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);

                if(user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

                    if(!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return RedirectToAction("Error");
                    }

                    return View("Success");
                }

                ModelState.AddModelError("", "Invalid Request");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAdress(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return View("Success");
                }
            }

            return RedirectToAction("Error");
        }
        

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Success()
        {
            return View("Success");
        }

    }
}
