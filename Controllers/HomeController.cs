using Electroscann_ai.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Electroscann_ai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
        public IActionResult Calculator()
        {
            return View();
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
