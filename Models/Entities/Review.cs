using System.ComponentModel.DataAnnotations;

namespace ElectroScanAI.Models.Entities
{
    public class Review : BaseEntity
    {
        public int ReviewerId { get; set; }

        public User Reviewer { get; set; }

        public int ElectricianId { get; set; }

        public Electrician Electrician { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }
    }
}   