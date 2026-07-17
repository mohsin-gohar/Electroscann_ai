using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Electroscann_ai.Data;
using Electroscann_ai.Models;
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

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalElectricians = await _context.Electricians.CountAsync(),
                TotalCompanies = await _context.Companies.CountAsync(),
                TotalJobs = await _context.Jobs.CountAsync(),
                TotalRevenue = await _context.Payments.Where(p => p.Status == PaymentStatus.Paid).SumAsync(p => p.Amount),
                PendingApplications = await _context.JobApplications.Where(ja => ja.Status == "Pending").CountAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Include(u => u.ElectricianProfile)
                .Include(u => u.CompanyProfile)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Electricians()
        {
            var electricians = await _context.Electricians
                .Include(e => e.User)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
            return View(electricians);
        }

        public async Task<IActionResult> Companies()
        {
            var companies = await _context.Companies
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(companies);
        }

        public async Task<IActionResult> Jobs()
        {
            var jobs = await _context.Jobs
                .Include(j => j.Company)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
            return View(jobs);
        }

        public async Task<IActionResult> Subscriptions() // FIXED: Added this action
        {
            var subscriptions = await _context.Subscriptions
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(subscriptions);
        }

        public async Task<IActionResult> AIScans()
        {
            var scans = await _context.AIScans
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(50)
                .ToListAsync();
            return View(scans);
        }

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

        public async Task<IActionResult> Messages() // FIXED: Spelling
        {
            var messages = await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return View(messages);
        }

        public IActionResult Settings()
        {
            return View();
        }

        public class AdminDashboardViewModel
        {
            public int TotalUsers { get; set; }
            public int TotalElectricians { get; set; }
            public int TotalCompanies { get; set; }
            public int TotalJobs { get; set; }
            public decimal TotalRevenue { get; set; }
            public int PendingApplications { get; set; }
        }

        public class MonthlyRevenue
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal Amount { get; set; }
        }
    }
}