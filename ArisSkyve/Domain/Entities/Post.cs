using Azure;
using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; }  
        public List<PostMedias> PostMedias { get; set; }
        public List<PostCategory> Categories { get; set; } = new();
        public List<Tags> Tags { get; set; } = new();
    }
}
