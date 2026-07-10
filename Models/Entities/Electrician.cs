namespace ElectroScanAI.Models.Entities
{
    public class Electrician
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string LicenseNumber { get; set; }
        public string Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsVerified { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public int CompletedJobs { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}