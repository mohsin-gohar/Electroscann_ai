using System.ComponentModel.DataAnnotations;
using ElectroScanAI.Models.Enums;

namespace ElectroScanAI.Models.Entities
{
    public class Job : BaseEntity
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Budget { get; set; }

        public string Location { get; set; }

        public JobStatus Status { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }
    }
}