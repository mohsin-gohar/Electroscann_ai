using System.ComponentModel.DataAnnotations;

namespace ElectroScanAI.Models.Entities
{
    public class MarketRate : BaseEntity
    {
        public string ItemName { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public string Unit { get; set; }

        public string City { get; set; }
    }
}