using ElectroScanAI.Models.Entities;
using Electroscann_ai.Data;
using Electroscann_ai.Models;
using Electroscann_ai.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Electroscann_ai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ElectroscannDbContext _context;

        public HomeController(ILogger<HomeController> logger, ElectroscannDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        public IActionResult Faq() => View();

        public IActionResult About() => View();

        public IActionResult AI_Scanner() => View();

        // ========== CONTACT GET ==========
        [HttpGet]
        public IActionResult Contact() => View();

        // ========== CONTACT POST — saves to DB ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string name, string email, string subject,
            string message, string? phone)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Name, Email, and Message are required.";
                return View();
            }

            _context.ContactMessages.Add(new ContactMessage
            {
                Name = name,
                Email = email,
                Subject = subject ?? string.Empty,
                Message = message,
                Phone = phone,
                IsResolved = false,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your message has been sent! We'll respond within 24 hours.";
            return RedirectToAction(nameof(Contact));
        }

        public IActionResult Service() => View();

        public IActionResult Testimonials() => View();

        // ========== WIRE COST CALCULATOR ==========
        [HttpGet]
        public async Task<IActionResult> Calculator()
        {
            var model = new WireCalculatorViewModel();
            await PopulateHistoryAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculator(WireCalculatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateHistoryAsync(model);
                return View(model);
            }

            ApplyCalculation(model);

            TryValidateModel(model);
            if (!ModelState.IsValid)
            {
                await PopulateHistoryAsync(model);
                return View(model);
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdClaim, out int userId) && userId > 0)
                {
                    _context.WireCalculations.Add(new WireCalculation
                    {
                        UserId = userId,
                        Load = model.Load,
                        Distance = model.Distance,
                        WireSize = model.WireSize ?? string.Empty,
                        Result = model.Result ?? string.Empty,
                        CreatedAt = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Calculation saved to your history.";
                }
            }
            else
            {
                TempData["Info"] = "Sign in to save this calculation to your history.";
            }

            await PopulateHistoryAsync(model);
            return View(model);
        }

        /// <summary>NEC-style rules matching site.js (voltage 220V).</summary>
        private static void ApplyCalculation(WireCalculatorViewModel model)
        {
            model.RoomArea = model.RoomLength * model.RoomWidth;
            double perimeter = 2 * (model.RoomLength + model.RoomWidth);
            model.Distance = Math.Round((perimeter * model.RoomHeight * 1.4) + (model.RoomArea * 0.9));

            model.Load = (model.Lights * 60)
                       + (model.Fans * 80)
                       + (model.Sockets * 150)
                       + (model.AcUnits * 2000)
                       + (model.HeavyLoads * 3000);

            model.CurrentAmps = Math.Round(model.Load / 220.0, 1);

            string gauge, breaker, safety;
            if (model.Load <= 1500)      { gauge = "14 AWG"; breaker = "15 A"; safety = "Standard residential wiring sufficient. Ensure proper grounding."; }
            else if (model.Load <= 2500) { gauge = "12 AWG"; breaker = "20 A"; safety = "12 AWG copper recommended. Suitable for general purpose circuits."; }
            else if (model.Load <= 3800) { gauge = "10 AWG"; breaker = "30 A"; safety = "Higher load detected. 10 AWG wire with 30A breaker recommended."; }
            else if (model.Load <= 5500) { gauge = "8 AWG";  breaker = "40 A"; safety = "Heavy load. Consider dedicated circuits for AC and appliances."; }
            else                         { gauge = "6 AWG";  breaker = "50 A"; safety = "High power demand. Consult a licensed electrician."; }

            model.WireSize = gauge;
            model.BreakerRating = breaker;
            model.SafetyMessage = safety;
            model.Result = $"Area: {model.RoomArea:0.#} sq ft | Current: {model.CurrentAmps:0.#} A | Breaker: {breaker} | {safety}";
            model.HasResult = true;
        }

        private async Task PopulateHistoryAsync(WireCalculatorViewModel model)
        {
            model.RecentCalculations = null;
            if (User.Identity?.IsAuthenticated != true) return;

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || userId <= 0) return;

            model.RecentCalculations = await _context.WireCalculations
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .Take(5)
                .Select(w => new WireCalculationHistoryItem
                {
                    Load = w.Load,
                    Distance = w.Distance,
                    WireSize = w.WireSize,
                    Result = w.Result,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();
        }

        public IActionResult Blog() => View();

        public IActionResult Careers() => View();

        public IActionResult Analytics() => View();

        public IActionResult System_Monitor() => View();

        public IActionResult Reports() => View();

        // ========== MARKET RATES — with real DB data + filtering ==========
        public async Task<IActionResult> Market_Rates(string? city, string? category)
        {
            var query = _context.MarketRates.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(m => m.City.Contains(city));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(m => m.Category == category);

            ViewBag.Rates = await query.OrderBy(m => m.Category).ThenBy(m => m.ItemName).ToListAsync();
            ViewBag.Cities = await _context.MarketRates.Select(m => m.City).Distinct().OrderBy(c => c).ToListAsync();
            ViewBag.Categories = await _context.MarketRates.Select(m => m.Category).Distinct().OrderBy(c => c).ToListAsync();
            ViewBag.FilterCity = city;
            ViewBag.FilterCategory = category;

            return View();
        }

        public IActionResult Demo() => View();

        public IActionResult Pricing() => View();

        public IActionResult Terms() => View();

        public IActionResult Cookies() => View();

        public IActionResult Estimation() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
