using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class BussinessAccount
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        [Required]
        public string ContactEmail { get; set; } = string.Empty;
        [Required]
        public string ContactPhone { get; set; } = string.Empty;
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
