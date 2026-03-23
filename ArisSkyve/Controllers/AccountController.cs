using ArisSkyve.Domain.Entities;
using ArisSkyve.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArisSkyve.Controllers
{
    public class AccountController:Controller
    {
        public ArisDBContext _context;
        public AccountController(ArisDBContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {
            var user = _context.User
                .FirstOrDefault(u => u.Email == model.Email);

            if (user != null && user.PasswordHash == model.PasswordHash)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
        [HttpPost]
        public async  Task<IActionResult> Register(User model)
        {
            if (true)
            {
                var existingUser = _context.User
                    .FirstOrDefault(u => u.Email == model.Email);
                if (existingUser == null)
                {
                    _context.User.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Email already in use.");
            }
            return View(model);
        }
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync();

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            var user = _context.User.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Email = email!,
                    Username = name!,
                    PasswordHash = ""
                };

                _context.User.Add(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult FacebookLogin()
        {
            var redirectUrl = Url.Action("FacebookResponse", "Account");

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync();

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            var user = _context.User.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Email = email!,
                    Username = name!,
                    PasswordHash = ""
                };

                _context.User.Add(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Dashboard");
        }

    }
}
