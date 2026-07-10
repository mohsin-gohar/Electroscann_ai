using System.ComponentModel.DataAnnotations;
using ElectroScanAI.Models.Enums;

namespace ElectroScanAI.Models.Entities
{
    public class AIScan : BaseEntity
    {
        [Required]
        public string ScanName { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string ImagePath { get; set; }

        public ScanRiskLevel RiskLevel { get; set; }

        public string Status { get; set; }
    }
}