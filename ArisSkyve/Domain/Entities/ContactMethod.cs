using System.ComponentModel.DataAnnotations;

namespace ArisSkyve.Domain.Entities
{
    public class ContactMethod
    {
        [Key]
        public int Id { get; set; }

        public string MethodType { get; set; } = string.Empty; 
        public string Value { get; set; } = string.Empty;

        public int ResumeId { get; set; }
        public Resume Resume { get; set; }
    }
}
