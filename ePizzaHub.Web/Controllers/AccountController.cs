using ePizzaHub.Core.Database.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Impelmentations;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ePizzaHub.Web.Controllers
{
    public class AccountController : Controller
    {
        IAuthService _authService;
        private readonly IUserRepository _user1;
        public AccountController(IAuthService authService, IUserRepository user1 = null)
        {
            _authService = authService;
            _user1 = user1;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model, string returnUrl)
        {
            UserModel user = _authService.ValidateUser(model.Email, model.Password);
            if (user != null)
            {
                GenerateTicket(user);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else if (user.Roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (user.Roles.Contains("User"))
                {
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
            }
            return View();
        }

        private async void GenerateTicket(UserModel user)
        {
            string strData = JsonSerializer.Serialize(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,string.Join(",",user.Roles)),
                new Claim(ClaimTypes.UserData,strData)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
            });
        }

        public IActionResult UnAuthorize()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        public IActionResult SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    CreatedDate = DateTime.Now,
                    EmailConfirmed = false
                   
                };
                
                _user1.CreateUser(user, model.Roles);
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }


    }
}
