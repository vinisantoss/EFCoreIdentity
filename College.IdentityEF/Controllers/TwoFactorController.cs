using College.Identity.Identity;
using College.IdentityEF.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace College.IdentityEF.Controllers
{

    public class TwoFactorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;

        public TwoFactorController(UserManager<User> userManager, IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory)
        {
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactor(TwoFactorModel twoFactor)
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Token expirado!");
                return RedirectToAction("Login", "Login");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(result.Principal.FindFirstValue("sub"));
                if(user != null)
                {
                   var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, result.Principal.FindFirstValue("amr"), twoFactor.Token);

                    if (isValid)
                    {
                        await HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);

                        var claimsPrincipal = await _userClaimsPrincipalFactory.CreateAsync(user);
                        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);

                        return RedirectToAction("about","Login");
                    }

                    ModelState.AddModelError("", "Token inválido");
                    return View();
                }

                ModelState.AddModelError("", "Request Inválida");
            }

            return View();
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
