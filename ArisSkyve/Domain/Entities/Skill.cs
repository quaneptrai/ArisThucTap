using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty; // Tên kỹ năng (VD: C#, React)

        // Khóa ngoại trỏ về Ứng viên
        public int EmployesAccountId { get; set; }
        public EmployesAccount EmployesAccount { get; set; }
    }
}