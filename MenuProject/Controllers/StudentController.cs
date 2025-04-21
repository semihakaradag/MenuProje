using MenuProject.Data;
using MenuProject.Models;
using MenuProject.Services;
using MenuProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenuProject.Controllers
{
    [CustomAuthorize("Student")]
    public class StudentController : Controller
    {
        private readonly MenuDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICourseService _courseService;

        public StudentController(MenuDbContext context, UserManager<IdentityUser> userManager, ICourseService courseService)
        {
            _context = context;
            _userManager = userManager;
            _courseService = courseService;
        }

        public IActionResult Courses()
        {
            return View();
        }
        public IActionResult Certificate()
        {
            return View();
        }
        public IActionResult Exams()
        {
            return View();
        }
        public IActionResult AllCourses()
        {
            var courses = _courseService.GetAllCourses(); // filtre yok, tüm kurslar
            return View(courses);
        }
        [HttpGet]
        public async Task<IActionResult> Info()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.AppUserId == user.Id);

            var model = new StudentInfoViewModel
            {
                Email = user.Email
            };

            if (student != null)
            {
                model.FirstName = student.FirstName;
                model.LastName = student.LastName;
                model.PhoneNumber = student.PhoneNumber;
                model.BirthDate = student.BirthDate;
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Info(StudentInfoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn", "Account");


            user.PhoneNumber = model.PhoneNumber;
            await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.AppUserId == user.Id);
            if (existingStudent != null)
            {
                existingStudent.FirstName = model.FirstName;
                existingStudent.LastName = model.LastName;
                existingStudent.PhoneNumber = model.PhoneNumber;
                existingStudent.BirthDate = model.BirthDate!.Value;

                await _context.SaveChangesAsync();
                return RedirectToAction("StudentDashboard", "Home");
            }

            var student = new Student
            {
                AppUserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                BirthDate = model.BirthDate!.Value // nullable ise .Value unutma
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("StudentDashboard", "Home");
        }
    }
}
