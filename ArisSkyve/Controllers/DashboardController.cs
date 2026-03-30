using ArisSkyve.Domain.Entities;
using ArisSkyve.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace ArisSkyve.Controllers
{
    public class DashboardController : Controller
    { 
        public ArisDBContext _context;
        public DashboardController(ArisDBContext context) {
            _context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = _context.Users
                .Include(u => u.Profile)
                .Include(u => u.Profile.Education)
                .FirstOrDefault(u => u.Id == int.Parse(userId));

            return View(user);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
