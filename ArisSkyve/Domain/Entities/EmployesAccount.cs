using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class EmployesAccount
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string fullName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; }
        public string Role { get; set; } = "User";
        public string? ResumeUrl { get; set; }
        public string? Headline { get; set; }
        public string? Location { get; set; }
        public string? AboutMe { get; set; }
        public string? PersonalLink { get; set; }

        // Khai báo quan hệ 1-Nhiều (1 Ứng viên có nhiều Kỹ năng, Kinh nghiệm...)
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<Experience> Experiences { get; set; } = new List<Experience>();
        public List<Education> Educations { get; set; } = new List<Education>();
        public List<Language> Languages { get; set; } = new List<Language>();
    }
}