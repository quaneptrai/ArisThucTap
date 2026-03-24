using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class Education
    {
        [Key]
        public int Id { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int EmployesAccountId { get; set; }
        public EmployesAccount EmployesAccount { get; set; }
    }
}