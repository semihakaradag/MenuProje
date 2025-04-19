using MenuProject.Areas.Admin.ViewModels;
using MenuProject.Data;
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
        public IActionResult MemberList()
        {
            var students = _context.Students.Include(x => x.AppUser).ToList();
            var teachers = _context.Teachers.Include(x => x.AppUser).ToList();

            var incompleteStudents = students.Where(s =>
                string.IsNullOrWhiteSpace(s.FirstName) ||
                string.IsNullOrWhiteSpace(s.LastName) ||
                s.BirthDate == DateTime.MinValue
            ).ToList();

            var incompleteTeachers = teachers.Where(t =>
                string.IsNullOrWhiteSpace(t.FirstName) ||
                string.IsNullOrWhiteSpace(t.LastName)
            ).ToList();

            var model = new MemberListViewModel
            {
                Students = students,
                IncompleteStudents = incompleteStudents,
                Teachers = teachers,
                IncompleteTeachers = incompleteTeachers
            };

            return View(model);
        }
    }
}
