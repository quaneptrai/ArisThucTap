using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class BussinessAccount : EmployesAccount
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;

        public string? CompanyAddress { get; set; }

        public string? Industry { get; set; }

        public string? Description { get; set; }
    }
}
