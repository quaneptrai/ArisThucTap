using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class Language
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // VD: Tiếng Anh
        public string Proficiency { get; set; } = string.Empty; // VD: IELTS 7.0

        public int EmployesAccountId { get; set; }
        public EmployesAccount EmployesAccount { get; set; }
    }
}