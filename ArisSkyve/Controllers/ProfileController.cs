using ArisSkyve.Application.DTOs;
using ArisSkyve.Domain.Entities;
using ArisSkyve.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArisSkyve.Controllers
{
    public class ProfileController : Controller
    {
        public ArisDBContext _context;
        public ProfileController(ArisDBContext dBContext) {
                _context = dBContext;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = _context.Users
                .Include(u => u.Profile)
                .ThenInclude(p => p.ContactMethods)
                .Include(u => u.Profile.Education)
                .FirstOrDefault(u => u.Id == int.Parse(userId));
            return View(user);
        }
        [HttpPost]
        public IActionResult AddContact(ContactMethod contact)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var profile = _context.Profiles
                .Include(p => p.ContactMethods)
                .FirstOrDefault(p => p.UserId == userId);

            if (profile != null)
            {
                var newContact = new ContactMethod
                {
                    MethodType = contact.MethodType,
                    Value = contact.Value,
                    idEmployesAccount = profile.Id 
                };

                _context.ContactMethods.Add(newContact);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBio([FromBody] UpdateBioDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdStr == null)
                return Unauthorized();

            int userId = int.Parse(userIdStr);

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            if (user.Profile == null)
            {
                user.Profile = new EmployesAccount
                {
                    UserId = user.Id,
                    FullName = user.Username ?? "User",
                    Location = "Chưa cập nhật"
                };
            }

            user.Profile.Bio = dto.Bio?.Trim();
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}
