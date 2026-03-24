using ArisSkyve.Domain.Entities;
using ArisSkyve.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ArisSkyve.Controllers
{
    [Authorize] // Bắt buộc phải đăng nhập mới được vào trang này
    public class ProfileController : Controller
    {
        private readonly ArisDBContext _context;

        public ProfileController(ArisDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Lấy Email của người dùng đang đăng nhập từ Cookie/Google
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Chui vào DB, lôi User đó ra, "cắp nách" theo cả Hồ sơ, Kỹ năng, Kinh nghiệm
            var currentUser = await _context.User
                            .Include(u => u.employesAccount)
                                .ThenInclude(e => e.Skills)
                            .Include(u => u.employesAccount)
                                .ThenInclude(e => e.Experiences)
                            .Include(u => u.employesAccount)
                                .ThenInclude(e => e.Educations)
                            .Include(u => u.employesAccount)
                                .ThenInclude(e => e.Languages)
                            .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser == null) return NotFound();

            // 3. Nếu User này mới đăng ký, chưa có EmployesAccount thì tự động tạo một cái trống
            if (currentUser.employesAccount == null)
            {
                currentUser.employesAccount = new EmployesAccount
                {
                    UserId = currentUser.Id,
                    fullName = currentUser.Username ?? "Chưa cập nhật tên"
                };
                _context.UserAccounts.Add(currentUser.employesAccount);
                await _context.SaveChangesAsync();
            }

            // 4. Gói ghém toàn bộ dữ liệu gửi sang View
            return View(currentUser);
        }
        [HttpPost]
        // BÍ KÍP NẰM Ở ĐÂY: Thêm 'string fullName' vào đầu cái ngoặc đơn
        public async Task<IActionResult> UpdateBasicInfo(string fullName, string headline, string location, string aboutMe, string personalLink)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var currentUser = await _context.User
                .Include(u => u.employesAccount)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser != null && currentUser.employesAccount != null)
            {
                // Cập nhật dữ liệu (Đã hết vạch đỏ nhé!)
                currentUser.employesAccount.fullName = fullName;
                currentUser.employesAccount.Headline = headline;
                currentUser.employesAccount.Location = location;
                currentUser.employesAccount.AboutMe = aboutMe;
                currentUser.employesAccount.PersonalLink = personalLink;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddSkill(string skillName)
        {
            // Kiểm tra xem có gõ chữ gì không, để trống thì văng về
            if (string.IsNullOrWhiteSpace(skillName)) return RedirectToAction("Index");

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            // Lấy User ra
            var currentUser = await _context.User
                .Include(u => u.employesAccount)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser != null && currentUser.employesAccount != null)
            {
                // Tạo một cục Kỹ Năng mới tinh
                var newSkill = new Skill
                {
                    Name = skillName.Trim(),
                    EmployesAccountId = currentUser.employesAccount.Id // Nối với ID của ứng viên này
                };

                // Thêm vào bảng Skills và Lưu
                _context.Skills.Add(newSkill);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddExperience(Experience exp)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var currentUser = await _context.User.Include(u => u.employesAccount).FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser?.employesAccount != null)
            {
                exp.EmployesAccountId = currentUser.employesAccount.Id;
                _context.Experiences.Add(exp);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddEducation(Education edu)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var currentUser = await _context.User.Include(u => u.employesAccount).FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser?.employesAccount != null)
            {
                edu.EmployesAccountId = currentUser.employesAccount.Id;
                _context.Educations.Add(edu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddLanguage(Language lang)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var currentUser = await _context.User.Include(u => u.employesAccount).FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser?.employesAccount != null)
            {
                lang.EmployesAccountId = currentUser.employesAccount.Id;
                _context.Languages.Add(lang);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UploadCV(IFormFile cvFile, [FromServices] IWebHostEnvironment env)
        {
            if (cvFile == null || cvFile.Length == 0) return RedirectToAction("Index");

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var currentUser = await _context.User.Include(u => u.employesAccount).FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser?.employesAccount != null)
            {
                // 1. Tạo thư mục chứa CV nếu chưa có
                string uploadsFolder = Path.Combine(env.WebRootPath, "uploads", "cvs");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                // 2. Mã hóa tên file để không bị trùng lặp
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + cvFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // 3. Copy file từ máy tính lên Server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await cvFile.CopyToAsync(fileStream);
                }

                // 4. Lưu đường dẫn vào Database
                currentUser.employesAccount.ResumeUrl = "/uploads/cvs/" + uniqueFileName;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}