using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class Resume
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<ContactMethod> ContactMethods { get; set; } = new List<ContactMethod>();
        public List<Skills> Skills { get; set; } = new List<Skills>();
        public List<Experiences> Experiences { get; set; } = new List<Experiences>();
        public List<Education> Educations { get; set; } = new List<Education>();
        public string? VectorId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
