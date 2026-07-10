using System.ComponentModel.DataAnnotations;

namespace ElectroScanAI.Models.Entities
{
    public class ScanResult : BaseEntity
    {
        public int AIScanId { get; set; }

        public AIScan AIScan { get; set; }

        public string DetectedIssue { get; set; }

        public string Severity { get; set; }

        public string Recommendation { get; set; }
    }
}