using ArisSkyve.Application.Dtos;
using ArisSkyve.Application.DTOs;
using ArisSkyve.Domain.Entities;
using ArisSkyve.Infrastructure.Data.Context;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        public IActionResult NewUserInfor(int userId)
        {
            var user = _context.Users.Include(u => u.Profile).FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            ViewBag.UserId = user.Id;
            ViewBag.Username = user.Username; 

            var dto = new NewUserInforDTO
            {
                Location = user.Profile?.Location,
                JobTitle = user.Profile?.JobTitle,
                IsStudent = user.Profile?.isStudent ?? false
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> NewUserInfor(NewUserInforDTO dto)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    var key = item.Key;
                    var errors = item.Value.Errors;

                    foreach (var error in errors)
                    {
                        Console.WriteLine($"{key}: {error.ErrorMessage}");
                    }
                }

                return View(dto);
            }

            var user = await _context.Users
                .Include(u => u.Profile)
                    .ThenInclude(p => p.Education)
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null) return NotFound();

            if (user.Profile == null)
            {
                user.Profile = new EmployesAccount { UserId = user.Id };
                _context.Profiles.Add(user.Profile);
            }

            user.Profile.Location = dto.Location;
            user.Profile.JobTitle = dto.JobTitle;
            user.Profile.isStudent = dto.IsStudent;
            user.Profile.IsLookingForJob = dto.JobSearchingStatus == "Looking";
            user.Profile.Bio = $"Muốn chức vụ: {dto.DesiredJobTitle}, nơi làm: {dto.DesiredCompany}";
            user.Profile.ShareWithRecruiters = dto.ShareWithRecruiters;
            user.Profile.IsRemoteJob = dto.IsRemoteJob;
            if (dto.IsStudent && dto.Education != null)
            {
                if (user.Profile.Education == null)
                {
                    user.Profile.Education = new Education { EmployesAccountId = user.Profile.Id };
                    _context.Add(user.Profile.Education);
                }

                user.Profile.Education.School = dto.Education.School;
                user.Profile.Education.Degree = dto.Education.Degree;
                user.Profile.Education.FieldOfStudy = dto.Education.FieldOfStudy;
                user.Profile.Education.StartYear = dto.Education.StartYear;
            }
            else if (user.Profile.Education != null)
            {
                _context.Remove(user.Profile.Education);
                user.Profile.Education = null;
            }

            await _context.SaveChangesAsync();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
             CookieAuthenticationDefaults.AuthenticationScheme,
             principal,
             new AuthenticationProperties
             {
                 IsPersistent = true,
                 ExpiresUtc = DateTime.UtcNow.AddDays(7)
             });
            return RedirectToAction("Index", "Dashboard", new { userId = user.Id });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var user = _context.Users
                .Include(u => u.Profile)
                .FirstOrDefault(u => u.Email == login.Email);
            
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email không tồn tại");
                return View(login);
            }

            if (user.PasswordHash != login.Password)
            {
                ModelState.AddModelError("Password", "Sai mật khẩu");
                return View(login);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            });
            return RedirectToAction("Index", "Dashboard", new { userId = user.Id });
        }
        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == model.Email);

            if (existingUser == null)
            {
                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("NewUserInfor", new { userId = model.Id });
            }

            ModelState.AddModelError("", "Email already in use.");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
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

            var user = _context.Users
                .Include(u => u.Profile)
                .FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Email = email!,
                    Username = name!,
                    PasswordHash = ""
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("NewUserInfor", new { userId = user.Id });
            }

            if (user.Profile == null)
            {
                return RedirectToAction("NewUserInfor", new { userId = user.Id });
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
             CookieAuthenticationDefaults.AuthenticationScheme,
             principal,
             new AuthenticationProperties
             {
                 IsPersistent = true,
                 ExpiresUtc = DateTime.UtcNow.AddDays(7)
             });
            return RedirectToAction("Index", "Dashboard", new { userId = user.Id });
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

            var user = _context.Users.Include(u => u.Profile).FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Email = email!,
                    Username = name!,
                    PasswordHash = ""
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("NewUserInfor", new { userId = user.Id });
            }
            if (user.Profile == null)
            {
                return RedirectToAction("NewUserInfor", new { userId = user.Id });
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            });
            return RedirectToAction("Index", "Dashboard", new { userId = user.Id });

        }

    }
}
