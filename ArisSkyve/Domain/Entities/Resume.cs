    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace ArisSkyve.Domain.Entities
    {
        public class Resume
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public int EmployeeId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string SearchVectorContent { get; set; } = string.Empty;
            public List<Skills> Skills { get; set; } = new List<Skills>();
            public List<Experiences> Experiences { get; set; } = new List<Experiences>();
            public List<Education> Educations { get; set; } = new List<Education>();
            public string? VectorId { get; set; }
            public string Category { get; set; } = string.Empty;
            public string Note { get; set; } = string.Empty;
            [ForeignKey("EmployeeId")]
            public virtual EmployesAccount Account { get; set; } = null!;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime LastUpdatedAt { get; set; }
        }
    }
