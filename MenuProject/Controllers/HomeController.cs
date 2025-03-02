using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MenuProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace MenuProject.Controllers;

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
    [Authorize(Roles = "Student")]
    public IActionResult StudentDashboard()
    {
        return View();
    }

    [Authorize(Roles = "Teacher")]
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
