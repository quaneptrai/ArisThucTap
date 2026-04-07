    using ArisSkyve.Infrastructure.Data.Context;
    using ArisSkyve.Domain.Entities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Claims;
using System.Text.Json;

[Authorize]
    public class JobsController : Controller
    {
    private readonly ArisDBContext _context;

    public JobsController(ArisDBContext context)
    {
        _context = context;
    }

    // Index chỉ load ban đầu vài job
    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return RedirectToAction("Login", "Account");

        int userId = int.Parse(userIdClaim);

        var user = await _context.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Education)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound();

        // Load trước 5 job
        var jobs = await _context.JobPostings
            .Include(j => j.Locations)
            .OrderByDescending(j => j.CreatedAt)
            .Take(5)
            .ToListAsync();

        var model = new JobsIndexViewModel
        {
            User = user,
            Profile = user.Profile,
            JobPostings = jobs
        };

        return View(model);
    }
    public async Task<IActionResult> Details(int id)
    {
        var job = await _context.JobPostings
            .Include(j => j.Locations)
            .Include(j => j.BusinessAccount)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            return NotFound();
        }

        // Parse JSON strings thành List<string> để hiển thị ở View
        // Sử dụng ?? "[]" để tránh lỗi nếu chuỗi trong DB bị null
        ViewBag.Responsibilities = JsonSerializer.Deserialize<List<string>>(job.Responsibilities ?? "[]");
        ViewBag.Requirements = JsonSerializer.Deserialize<List<string>>(job.Requirements ?? "[]");
        ViewBag.Benefits = JsonSerializer.Deserialize<List<string>>(job.Benefits ?? "[]");

        return View(job);
    }
    // API load thêm jobs
    [HttpGet]
    public async Task<IActionResult> GetJobs(int skip = 0, int take = 5)
    {
        var jobs = await _context.JobPostings
            .Include(j => j.Locations)
            .OrderByDescending(j => j.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        if (!jobs.Any())
            return Json(new { success = false });

        return PartialView("_JobCardPartial", jobs);
    }
}

    // ViewModel
    public class JobsIndexViewModel
    {
        public ArisSkyve.Domain.Entities.User User { get; set; } = default!;
        public ArisSkyve.Domain.Entities.EmployesAccount Profile { get; set; } = default!;
    public List<ArisSkyve.Domain.Entities.JobPosting> JobPostings { get; set; } = new List<ArisSkyve.Domain.Entities.JobPosting>();
    }