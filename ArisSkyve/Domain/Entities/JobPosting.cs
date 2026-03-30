using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArisSkyve.Domain.Entities
{
    public class JobPosting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Company { get; set; } = string.Empty;

        // JSON fields
        public string Locations { get; set; } = "[]"; // store JSON array
        public string Responsibilities { get; set; } = "[]"; // JSON array
        public string Requirements { get; set; } = "[]"; // JSON array
        public string Benefits { get; set; } = "[]"; // JSON array
        public string? Tags { get; set; } = "{}"; // JSON object { "Yêu cầu":[], "Quyền lợi":[], "Chuyên môn":[] }

        [MaxLength(200)]
        public string WorkTime { get; set; } = string.Empty;

        public string FullText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // optional: store vector for hybrid search
        public string? FullTextEmbedding { get; set; }
        public int? BusinessAccountId { get; set; }
        public BussinessAccount? BusinessAccount { get; set; }
    }
}
