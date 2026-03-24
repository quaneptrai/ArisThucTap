using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class Experience
    {
        [Key]
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }

        public int EmployesAccountId { get; set; }
        public EmployesAccount EmployesAccount { get; set; }
    }
}