using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuProject.Controllers
{
    [CustomAuthorize("Student")]
    public class StudentController : Controller
    {
        
        public IActionResult Courses()
        {
            return View();
        }
        public IActionResult Exams()
        {
            return View();
        }
    }
}
