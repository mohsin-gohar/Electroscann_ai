using System.ComponentModel.DataAnnotations;

namespace ElectroScanAI.Models.Entities
{
    public class ContactMessage : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public bool IsResolved { get; set; }
    }
}