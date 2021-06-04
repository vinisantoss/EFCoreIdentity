using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using College.Identity.Identity;
using College.Identity.Models;
using College.IdentityEF.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace College.Identity.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;

        public LoginController(ILogger<LoginController> logger, 
            UserManager<User> userManager,
            IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory)
        {
            _userManager = userManager;
            _logger = logger;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(login.UserName);

                if(user != null &&  !await _userManager.IsLockedOutAsync(user))
                {
                    if (await _userManager.CheckPasswordAsync(user, login.Password))
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            ModelState.AddModelError("", "E-mail não está valido, você precisa confirmar o seu e-mail!");
                            return View();
                        }

                        await _userManager.ResetAccessFailedCountAsync(user);

                        if(await _userManager.GetTwoFactorEnabledAsync(user))
                        {
                            var validator = await _userManager.GetValidTwoFactorProvidersAsync(user);

                            if(validator.Contains("Email"))
                            {
                                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                                System.IO.File.WriteAllText("Email2sv.txt", token);

                                await HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme,
                                    new Store2FA().CreateStore2FA(user.Id, "Email"));

                                return RedirectToAction("TwoFactor", "TwoFactor");
                            }
                        }

                        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

                        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(principal));

                        return RedirectToAction("About");
                    }

                    await _userManager.AccessFailedAsync(user);

                    if(await _userManager.IsLockedOutAsync(user))
                    {
                        //enviar email como sugestão para mudança de senha
                    }
                    ModelState.AddModelError("", "Usuário ou Senha Inválida!");
                }
                
            }

            /*
             
              var signInResult = await _signInManager.PasswordSignInAsync(
                    login.UserName, login.Password, false, false); // penultimo verifica se mantem o cookie persistente depois que o browser fecha, o ultimo bloqueia se ele errar o login

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("About");
                }
*/

            return View(); 
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> About()
        {
            return View();
        }
    }
}
