using System.ComponentModel.DataAnnotations;

namespace ElectroScanAI.Models.Entities
{
    public class ActivityLog : BaseEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public string Action { get; set; }

        public string Description { get; set; }

        public string IPAddress { get; set; }
    }
}