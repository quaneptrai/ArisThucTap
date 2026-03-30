using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string role { get; set; } = "Employee";

        public EmployesAccount? Profile { get; set; }
        public List<Post> Posts { get; set; } = new();
    }
}
