using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Electroscann_ai.Data;
using ElectroScanAI.Models.Enums;

namespace Electroscann_ai.Controllers
{
    [Authorize(Roles = "Electrician")]
    public class ElectricianDashboardController : Controller
    {
        private readonly ElectroscannDbContext _context;

        public ElectricianDashboardController(ElectroscannDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var electrician = await _context.Electricians
                .FirstOrDefaultAsync(e => e.UserId == userId);

            ViewBag.AvailableJobs = await _context.Jobs
                .CountAsync(j => j.Status == JobStatus.Open);

            ViewBag.CompletedJobs = electrician?.CompletedJobs ?? 0;
            ViewBag.Rating = electrician?.Rating ?? 0;

            return View();
        }

        public IActionResult Profile() => View();
        public IActionResult AvailableJobs() => View();
        public IActionResult AppliedJobs() => View();
        public IActionResult Projects() => View();
        public IActionResult Earnings() => View();
        public IActionResult Messages() => View();
        public IActionResult Notifications() => View();
        public IActionResult Settings() => View();
    }
}