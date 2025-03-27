using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MenuProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MenuProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Home"); // Herkes ortak Dashboard'a gidecek
            }
            return View(); // Eðer giriþ yapýlmamýþsa Welcome sayfasýný göster
        }

        [Authorize] // Kullanýcý giriþi olmadan eriþilemez!
        public IActionResult Dashboard()
        {
            return View(); // Herkes ayný Dashboard sayfasýna gidiyor ama menü rollere göre deðiþiyor!
        }

        public IActionResult StudentDashboard()
        {
            return View();

        }

        public IActionResult TeacherDashboard()
        {
            return View();
        }

        public IActionResult Privacy()
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
