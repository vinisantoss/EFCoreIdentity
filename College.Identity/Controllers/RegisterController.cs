using College.Identity.Identity;
using College.Identity.Models;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                        UserName = register.UserName
                    };

                    var result = await _userManager.CreateAsync(user, register.Password);

                    if (result.Errors.Any())
                    {
                        return View("Error");
                    }

                }
            }

            return View("Success");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }
    }
}
