using ElectroScanAI.Models.Entities;
using Electroscann_ai.Models;
using Microsoft.EntityFrameworkCore;

namespace Electroscann_ai.Data
{
    public class ElectroscannDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ElectroscannDbContext(DbContextOptions<ElectroscannDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Electrician> Electricians { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<AIScan> AIScans { get; set; }
        public DbSet<ScanResult> ScanResults { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WireCalculation> WireCalculations { get; set; }
        public DbSet<MarketRate> MarketRates { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}