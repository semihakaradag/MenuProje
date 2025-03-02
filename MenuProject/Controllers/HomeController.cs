using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MenuProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MenuProject.Controllers;

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
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Student"))
                {
                    return RedirectToAction("StudentDashboard", "Home");
                }
                else if (roles.Contains("Teacher"))
                {
                    return RedirectToAction("TeacherDashboard", "Home");
                }
                else if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
            }
        }

        return View(); // Eðer giriþ yapýlmamýþsa Welcome sayfasýný göster
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [CustomAuthorize("Student")]
    public IActionResult StudentDashboard()
    {
        return View();
    }

    [CustomAuthorize("Teacher")]
    public IActionResult TeacherDashboard()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AdminDashboard()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
