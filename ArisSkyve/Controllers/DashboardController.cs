using ArisSkyve.Domain.Entities;
using ArisSkyve.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace ArisSkyve.Controllers
{
    public class DashboardController : Controller
    {
        public ArisDBContext _context;
        public DashboardController(ArisDBContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Lấy ID từ Claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            // Lấy thông tin User kèm Profile và danh sách Posts (Sắp xếp mới nhất lên đầu)
            var user = _context.Users
                .Include(u => u.Profile)
                .Include(u => u.Profile.Education)
                .FirstOrDefault(u => u.Id == userId);
            var posts = await _context.Posts
                            .Include(p => p.User)
                                .ThenInclude(u => u.Profile)
                            .OrderByDescending(p => p.CreatedAt)
                            .Take(10)
                            .ToListAsync();
            ViewBag.Posts = posts;
            return View(user);
        }

        [HttpPost]
        [Authorize]
        // Bổ sung string title vào tham số nhận từ AJAX
        public async Task<IActionResult> CreatePost(string content, string title)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Json(new { success = false, message = "User not authenticated" });

                int userId = int.Parse(userIdClaim);

                var post = new Post
                {
                    Content = content,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsPublic = true,
                    // FIX TẠI ĐÂY: Nếu có title thì lưu title, không thì để null hoặc rỗng
                    Title = !string.IsNullOrEmpty(title) ? title : null
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                var postWithUser = await _context.Posts
                    .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                    .FirstOrDefaultAsync(p => p.Id == post.Id);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = postWithUser.Id,
                        username = postWithUser.User.Username,
                        avatar = postWithUser.User.Profile?.AvatarUrl ?? "/images/default-avatar.png",
                        content = postWithUser.Content,
                        title = postWithUser.Title, // Gửi title về lại cho JS
                        time = "Vừa xong",
                        isArticle = !string.IsNullOrEmpty(postWithUser.Title) // Báo cho JS biết đây là Article
                    }
                });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
        }

        // THÊM HÀM XÓA BÀI VIẾT
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Json(new { success = false });

            int userId = int.Parse(userIdClaim);

            // Tìm bài viết và check xem có đúng chủ sở hữu không
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (post == null)
            {
                return Json(new { success = false, message = "Bài viết không tồn tại hoặc bạn không có quyền xóa." });
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
