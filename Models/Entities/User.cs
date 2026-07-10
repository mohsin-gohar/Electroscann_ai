using ElectroScanAI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ElectroScanAI.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool EmailVerified { get; set; }
        public string? ProfileImage { get; set; }

        // Navigation
        public Electrician? ElectricianProfile { get; set; }
        public Company? CompanyProfile { get; set; }
    }
}