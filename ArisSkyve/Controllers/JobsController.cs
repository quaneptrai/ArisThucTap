using ArisSkyve.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class JobsController : Controller
{
    private readonly ArisDBContext _context;

    public JobsController(ArisDBContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy userId từ claims (cookie auth)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return RedirectToAction("Login", "Account");

        int userId = int.Parse(userIdClaim);

        // Lấy user từ database
        var user = await _context.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Education)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound();

        return View(user); // truyền user sang view
    }
}