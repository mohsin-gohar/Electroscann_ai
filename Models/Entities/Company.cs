namespace ElectroScanAI.Models.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; }
        public string NTNNumber { get; set; }
        public string Industry { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Logo { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}