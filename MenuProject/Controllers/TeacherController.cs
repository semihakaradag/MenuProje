using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuProject.Controllers
{
    [CustomAuthorize("Teacher")]
    public class TeacherController : Controller
    {
        public IActionResult Classes()
        {
            return View();
        }
        public IActionResult Students()
        {
            return View();
        }

    }
}
