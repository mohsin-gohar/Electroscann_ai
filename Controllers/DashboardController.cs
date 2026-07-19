using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Electroscann_ai.Data;
using Electroscann_ai.Models;
using ElectroScanAI.Models.Entities;
using ElectroScanAI.Models.Enums;

namespace Electroscann_ai.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ElectroscannDbContext _context;

        public DashboardController(ElectroscannDbContext context)
        {
            _context = context;
        }

        // ===== INDEX (Admin Dashboard Home) =====
        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(u => !u.IsDeleted),
                TotalElectricians = await _context.Electricians.CountAsync(),
                TotalCompanies = await _context.Companies.CountAsync(),
                TotalJobs = await _context.Jobs.CountAsync(),
                TotalRevenue = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Paid)
                    .SumAsync(p => p.Amount),
                PendingApplications = await _context.JobApplications
                    .Where(ja => ja.Status == "Pending").CountAsync(),
                TotalScans = await _context.AIScans.CountAsync(),
                ActiveSubscriptions = await _context.Subscriptions
                    .Where(s => s.IsActive).CountAsync(),
                UnreadMessages = await _context.ContactMessages
                    .Where(m => !m.IsResolved).CountAsync()
            };
            return View(viewModel);
        }

        // ===== USERS LIST =====
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Include(u => u.ElectricianProfile)
                .Include(u => u.CompanyProfile)
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View(users);
        }

        // ===== TOGGLE USER ACTIVE =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserActive(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User {(user.IsActive ? "activated" : "deactivated")}.";
            return RedirectToAction(nameof(Users));
        }

        // ===== SOFT DELETE USER =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsDeleted = true;
            user.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "User deleted.";
            return RedirectToAction(nameof(Users));
        }

        // ===== ELECTRICIANS =====
        public async Task<IActionResult> Electricians()
        {
            var electricians = await _context.Electricians
                .Include(e => e.User)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
            return View(electricians);
        }

        // ===== VERIFY ELECTRICIAN =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyElectrician(int id)
        {
            var electrician = await _context.Electricians.FindAsync(id);
            if (electrician == null) return NotFound();

            electrician.IsVerified = !electrician.IsVerified;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Electrician {(electrician.IsVerified ? "verified" : "unverified")}.";
            return RedirectToAction(nameof(Electricians));
        }

        // ===== COMPANIES =====
        public async Task<IActionResult> Companies()
        {
            var companies = await _context.Companies
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(companies);
        }

        // ===== VERIFY COMPANY =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null) return NotFound();

            company.IsVerified = !company.IsVerified;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Company {(company.IsVerified ? "verified" : "unverified")}.";
            return RedirectToAction(nameof(Companies));
        }

        // ===== JOBS =====
        public async Task<IActionResult> Jobs()
        {
            var jobs = await _context.Jobs
                .Include(j => j.Company)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
            return View(jobs);
        }

        // ===== DELETE JOB =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job deleted.";
            return RedirectToAction(nameof(Jobs));
        }

        // ===== SUBSCRIPTIONS =====
        public async Task<IActionResult> Subscriptions()
        {
            var subscriptions = await _context.Subscriptions
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(subscriptions);
        }

        // ===== AI SCANS =====
        public async Task<IActionResult> AIScans()
        {
            var scans = await _context.AIScans
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(50)
                .ToListAsync();
            return View(scans);
        }

        // ===== REVENUE =====
        public async Task<IActionResult> Revenue()
        {
            var revenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Paid)
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => new MonthlyRevenue
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Amount = g.Sum(p => p.Amount)
                })
                .OrderByDescending(r => r.Year)
                .ThenByDescending(r => r.Month)
                .ToListAsync();
            return View(revenue);
        }

        // ===== CONTACT MESSAGES =====
        public async Task<IActionResult> Messages()
        {
            var messages = await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return View(messages);
        }

        // ===== RESOLVE MESSAGE =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveMessage(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg == null) return NotFound();

            msg.IsResolved = true;
            msg.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Message marked as resolved.";
            return RedirectToAction(nameof(Messages));
        }

        // ===== DELETE MESSAGE =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg == null) return NotFound();

            _context.ContactMessages.Remove(msg);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Message deleted.";
            return RedirectToAction(nameof(Messages));
        }

        // ===== SETTINGS =====
        public IActionResult Settings() => View();

        // ===== VIEW MODELS =====
        public class AdminDashboardViewModel
        {
            public int TotalUsers { get; set; }
            public int TotalElectricians { get; set; }
            public int TotalCompanies { get; set; }
            public int TotalJobs { get; set; }
            public decimal TotalRevenue { get; set; }
            public int PendingApplications { get; set; }
            public int TotalScans { get; set; }
            public int ActiveSubscriptions { get; set; }
            public int UnreadMessages { get; set; }
        }

        public class MonthlyRevenue
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal Amount { get; set; }
        }
    }
}