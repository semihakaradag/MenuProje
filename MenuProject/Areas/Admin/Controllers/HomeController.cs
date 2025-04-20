using MenuProject.Areas.Admin.ViewModels;
using MenuProject.Data;
using MenuProject.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenuProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly MenuDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public HomeController(MenuDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult AdminDashboard()
        {
            return View(); 
        }


        [ServiceFilter(typeof(IncompleteUserFilterAttribute))]
        public IActionResult MemberList()
        {
            var students = _context.Students.Include(s => s.AppUser).ToList();
            var teachers = _context.Teachers.Include(t => t.AppUser).ToList();

            var incompleteStudents = students.Where(s => string.IsNullOrEmpty(s.PhoneNumber)).ToList();
            var incompleteTeachers = teachers.Where(t => string.IsNullOrEmpty(t.PhoneNumber)).ToList();

            var model = new MemberListViewModel
            {
                Students = students,
                Teachers = teachers,
                IncompleteStudents = incompleteStudents,
                IncompleteTeachers = incompleteTeachers
            };

            return View(model);
        }
    }
}
