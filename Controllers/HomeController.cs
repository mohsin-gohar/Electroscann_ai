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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult AI_Scanner()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Service()
        {
            return View();
        }

        public IActionResult Testimonials()
        {
            return View();
        }

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

            // Re-validate computed Load / Distance before any DB write
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
                    var entity = new WireCalculation
                    {
                        UserId = userId,
                        Load = model.Load,
                        Distance = model.Distance,
                        WireSize = model.WireSize ?? string.Empty,
                        Result = model.Result ?? string.Empty,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.WireCalculations.Add(entity);
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

        /// <summary>
        /// Same NEC-style rules as wwwroot/js/site.js (voltage 220V).
        /// </summary>
        private static void ApplyCalculation(WireCalculatorViewModel model)
        {
            model.RoomArea = model.RoomLength * model.RoomWidth;

            double perimeter = 2 * (model.RoomLength + model.RoomWidth);
            model.Distance = Math.Round((perimeter * model.RoomHeight * 1.4) + (model.RoomArea * 0.9));

            const int lightW = 60;
            const int fanW = 80;
            const int socketW = 150;
            const int acW = 2000;
            const int heavyW = 3000;

            model.Load = (model.Lights * lightW)
                       + (model.Fans * fanW)
                       + (model.Sockets * socketW)
                       + (model.AcUnits * acW)
                       + (model.HeavyLoads * heavyW);

            const double voltage = 220;
            model.CurrentAmps = Math.Round(model.Load / voltage, 1);

            string gauge;
            string breaker;
            string safety;

            if (model.Load <= 1500)
            {
                gauge = "14 AWG";
                breaker = "15 A";
                safety = "Standard residential wiring sufficient. Ensure proper grounding.";
            }
            else if (model.Load <= 2500)
            {
                gauge = "12 AWG";
                breaker = "20 A";
                safety = "12 AWG copper recommended. Suitable for general purpose circuits.";
            }
            else if (model.Load <= 3800)
            {
                gauge = "10 AWG";
                breaker = "30 A";
                safety = "Higher load detected. 10 AWG wire with 30A breaker recommended.";
            }
            else if (model.Load <= 5500)
            {
                gauge = "8 AWG";
                breaker = "40 A";
                safety = "Heavy load. Consider dedicated circuits for AC and appliances.";
            }
            else
            {
                gauge = "6 AWG";
                breaker = "50 A";
                safety = "High power demand. Consult a licensed electrician.";
            }

            model.WireSize = gauge;
            model.BreakerRating = breaker;
            model.SafetyMessage = safety;
            model.Result =
                $"Area: {model.RoomArea:0.#} sq ft | Current: {model.CurrentAmps:0.#} A | Breaker: {breaker} | {safety}";
            model.HasResult = true;
        }

        private async Task PopulateHistoryAsync(WireCalculatorViewModel model)
        {
            model.RecentCalculations = null;

            if (User.Identity?.IsAuthenticated != true)
                return;

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId) || userId <= 0)
                return;

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

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult Careers()
        {
            return View();
        }

        public IActionResult Analytics()
        {
            return View();
        }

        public IActionResult System_Monitor()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult Market_Rates()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Demo()
        {
            return View();
        }

        public IActionResult Pricing()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Cookies()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
