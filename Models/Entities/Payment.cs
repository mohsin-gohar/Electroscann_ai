using System.ComponentModel.DataAnnotations;
using ElectroScanAI.Models.Enums;

namespace ElectroScanAI.Models.Entities
{
    public class Payment : BaseEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public decimal Amount { get; set; }

        public string TransactionId { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}