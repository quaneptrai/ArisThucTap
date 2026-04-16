using ArisSkyve.Application.DTOs;
using ArisSkyve.Domain.Entities;
using ArisSkyve.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArisSkyve.Controllers
{

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdClaim);

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var jobs = await _context.JobPostings
                .OrderByDescending(j => j.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Lúc này JobsIndexViewModel đã được nhận diện
            var model = new JobsIndexViewModel
            {
                User = user,
                Profile = user?.Profile,
                JobPostings = jobs,
                RecommendedJobs = new List<JobRecommendationDTO>()
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobPostings
            .FirstOrDefaultAsync(j => j.Id == id);
            System.Diagnostics.Debug.WriteLine($"DEBUG: Job ID {id} - Title: {job?.Title}");
            System.Diagnostics.Debug.WriteLine($"DEBUG: Responsibilities: {job?.Responsibilities ?? "NULL"}");
            if (job == null) return NotFound();

            return View(job);
        }
        [HttpGet]
        public async Task<IActionResult> GetAiRecommendations(int topK = 3)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Content("");
            int currentUserId = int.Parse(userIdClaim);

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == currentUserId);
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.EmployeeId == profile.Id);

            if (profile == null || resume == null) return Content("<p>Vui lòng cập nhật hồ sơ.</p>");

            DateTime currentResumeVersion = resume.LastUpdatedAt;

            // 1. Kiểm tra xem trong DB đã có bao nhiêu gợi ý cho version Resume này rồi
            var existingRecs = await _context.UserRecommendations
                .Where(r => r.UserId == currentUserId && r.ResumeVersion == currentResumeVersion)
                .OrderByDescending(r => r.MatchScore)
                .ToListAsync();

            // 2. Nếu số lượng trong DB đã đủ (hoặc nhiều hơn) topK yêu cầu -> Trả về luôn, không gọi AI nữa
            if (existingRecs.Count >= topK)
            {
                var savedJobIds = existingRecs.Take(topK).Select(r => r.JobId).ToList();
                var cachedJobs = await _context.JobPostings
                    .Where(j => savedJobIds.Contains(j.Id))
                    .ToListAsync();

                // Map lại đúng Score và Reason từ existingRecs
                var resultDto = cachedJobs.Select(j => new JobRecommendationDTO
                {
                    Id = j.Id,
                    JobTitle = j.Title,
                    Score = existingRecs.First(r => r.JobId == j.Id).MatchScore,
                    Reason = existingRecs.First(r => r.JobId == j.Id).Reason
                }).OrderByDescending(x => x.Score).ToList();

                return PartialView("_RecommendedJobsPartial", resultDto);
            }

            // 3. Nếu DB chưa đủ topK -> Gọi AI để lấy thêm
            try
            {
                using (var client = new HttpClient())
                {
                    // Gửi topK lớn hơn để AI gợi ý thêm những cái mới
                    var nvc = new List<KeyValuePair<string, string>> {
                        new("employee_id", profile.Id.ToString()),
                        new("top_k", topK.ToString())
                        };

                    var response = await client.PostAsync("http://127.0.0.1:8001/recommend-jobs-for-employee", new FormUrlEncodedContent(nvc));

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<AiResponseWrapper>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (result?.Recommendations != null)
                        {
                            foreach (var rec in result.Recommendations)
                            {
                                // Chỉ thêm vào DB nếu JobId này chưa tồn tại cho User này + Version này
                                var jobInDb = await _context.JobPostings.FirstOrDefaultAsync(j => j.Title == rec.JobTitle);
                                if (jobInDb != null)
                                {
                                    var isExisted = await _context.UserRecommendations
                                        .AnyAsync(r => r.UserId == currentUserId && r.JobId == jobInDb.Id && r.ResumeVersion == currentResumeVersion);

                                    if (!isExisted)
                                    {
                                        _context.UserRecommendations.Add(new UserRecommendations
                                        {
                                            UserId = currentUserId,
                                            JobId = jobInDb.Id,
                                            MatchScore = rec.Score,
                                            Reason = rec.Reason,
                                            CreatedAt = DateTime.UtcNow,
                                            ResumeVersion = currentResumeVersion
                                        });
                                    }
                                    rec.Id = jobInDb.Id;
                                }
                            }
                            await _context.SaveChangesAsync();
                            return PartialView("_RecommendedJobsPartial", result.Recommendations.Take(topK).ToList());
                        }
                    }
                }
            }
            catch (Exception ex) { /* Log error */ }

            return Content("<p>Hệ thống đang tìm kiếm thêm...</p>");
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs(int skip = 0, int take = 5)
        {
            var jobs = await _context.JobPostings
                .Include(j => j.Locations)
                .OrderByDescending(j => j.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            if (!jobs.Any()) return Json(new { success = false });

            return PartialView("_JobCardPartial", jobs);
        }
        
     }
 }