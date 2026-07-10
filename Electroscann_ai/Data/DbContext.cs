using Electroscann_ai.Models;
using Microsoft.EntityFrameworkCore;

namespace Electroscann_ai.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ElectroScanAI.Models.Entities.User> Users { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.Electrician> Electricians { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.Company> Companies { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.Job> Jobs { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.JobApplication> JobApplications { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.AIScan> AIScans { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.ScanResult> ScanResults { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.Subscription> Subscriptions { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.Payment> Payments { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.Message> Messages { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.Notification> Notifications { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.Review> Reviews { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.WireCalculation> WireCalculations { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.MarketRate> MarketRates { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.ContactMessage> ContactMessages { get; set; }
        public DbSet<ElectroScanAI.Models.Entities.ActivityLog> ActivityLogs { get; set; }
    }
}