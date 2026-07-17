using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Electroscann_ai.Data;

namespace Electroscann_ai.Controllers
{
    [Authorize(Roles = "Company")]
    public class CompanyDashboardController : Controller
    {
        private readonly ElectroscannDbContext _context;

        public CompanyDashboardController(ElectroscannDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();
        public IActionResult Profile() => View();
        public IActionResult Jobs() => View();
        public IActionResult Applications() => View();
        public IActionResult Payments() => View();
        public IActionResult Settings() => View();
    }
}