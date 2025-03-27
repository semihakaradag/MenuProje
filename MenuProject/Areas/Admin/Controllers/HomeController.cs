using Microsoft.AspNetCore.Mvc;

namespace MenuProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult AdminDashboard()
        {
            return View(); 
        }
    }
}
