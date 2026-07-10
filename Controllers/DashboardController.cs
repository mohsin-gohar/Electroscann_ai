using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Electroscann_ai.Data;

namespace Electroscann_ai.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> companies()
        {
            var companiesData = await _context.Companies.ToListAsync();
            return View(companiesData);
        }

        public async Task<IActionResult> electrician()
        {
            var electriciansData = await _context.Electricians.ToListAsync();
            return View(electriciansData);
        }

        public async Task<IActionResult> jobs()
        {
            var jobsData = await _context.Jobs.ToListAsync();
            return View(jobsData);
        }

        public async Task<IActionResult> messaage()
        {
            var messagesData = await _context.Messages.ToListAsync();
            return View(messagesData);
        }

        public async Task<IActionResult> revenue()
        {
            var paymentsData = await _context.Payments.ToListAsync();
            return View(paymentsData);
        }

        public async Task<IActionResult> ai_scans()
        {
            var scansData = await _context.AIScans.ToListAsync();
            return View(scansData);
        }

        public async Task<IActionResult> user()
        {
            var usersData = await _context.Users.ToListAsync();
            return View(usersData);
        }

        public IActionResult settings()
        {
            return View();
        }
    }
}