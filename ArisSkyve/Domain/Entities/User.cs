using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public EmployesAccount employesAccount { get; set; }
        public BussinessAccount bussinessAccount { get; set; }
        public List<Post> Posts { get; set; }
    }
}