using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Electroscann_ai.Data;

namespace Electroscann_ai.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientDashboardController : Controller
    {
        private readonly ElectroscannDbContext _context;

        public ClientDashboardController(ElectroscannDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();
        public IActionResult Profile() => View();
        public IActionResult Projects() => View();
        public IActionResult Payments() => View();
        public IActionResult Settings() => View();
    }
}