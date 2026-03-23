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
    }
}
