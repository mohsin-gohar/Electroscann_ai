using System.ComponentModel.DataAnnotations;
using ElectroScanAI.Models.Enums;

namespace ElectroScanAI.Models.Entities
{
    public class Subscription : BaseEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public SubscriptionPlan Plan { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}