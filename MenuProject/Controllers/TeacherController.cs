using MenuProject.Data;
using MenuProject.Helpers;
using MenuProject.Models;
using MenuProject.Services;
using MenuProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenuProject.Controllers
{
    [CustomAuthorize("Teacher", "Admin")]
    public class TeacherController : Controller
    {
        private readonly MenuDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICourseService _courseService;

        public TeacherController(MenuDbContext context, UserManager<IdentityUser> userManager, ICourseService courseService)
        {
            _context = context;
            _userManager = userManager;
            _courseService = courseService;
        }

        //// **📌 1. Dersleri Listeleme**
        //public async Task<IActionResult> Classes()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var lessons = await _context.Lessons
        //        .Where(l => l.TeacherId == user.Id || User.IsInRole("Admin")) // Öğretmen sadece kendi eklediklerini görsün
        //        .ToListAsync();

        //    return View(lessons);
        //}

        //// **📌 2. Yeni Ders Ekleme (GET)**
        //[HttpGet]
        //public IActionResult AddClass()
        //{
        //    return View();
        //}

        //// **📌 3. Yeni Ders Ekleme (POST)**
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddClass(Lesson lesson)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        lesson.TeacherId = user.Id;

        //        _context.Lessons.Add(lesson);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction("Classes");
        //    }
        //    return View(lesson);
        //}

        //// **📌 4. Ders Silme**
        //[HttpPost]
        //public async Task<IActionResult> DeleteClass(int id)
        //{
        //    var lesson = await _context.Lessons.FindAsync(id);
        //    if (lesson == null) return NotFound();

        //    _context.Lessons.Remove(lesson);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Classes");
        //}

        //// **📌 5. Öğrencileri Listeleme**
        //public IActionResult Students()
        //{
        //    return View(); // Burada ilerleyen süreçte öğrenci işlemleri olacak
        //}

        public IActionResult Classes()
        {
            var courses = _courseService.GetAllCourses();
            return View(courses);
        }

        [ServiceFilter(typeof(MissingTeacherInfoFilter))]
        [HttpGet]
        public IActionResult AddCourse()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                _courseService.AddCourse(course);
                return RedirectToAction("Classes");
            }

            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> Info()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["Error"] = "Oturum bilgisi alınamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Account");
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.AppUserId == user.Id);

            var model = new TeacherInfoViewModel
            {
                Email = user.Email
            };

            if (teacher != null)
            {
                model.FirstName = teacher.FirstName;
                model.LastName = teacher.LastName;
                model.PhoneNumber = teacher.PhoneNumber;
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Info(TeacherInfoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

           
            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                await _userManager.SetPhoneNumberAsync(user, null);
            }
            else
            {
                await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            }

          
            var existing = await _context.Teachers.FirstOrDefaultAsync(t => t.AppUserId == user.Id);
            if (existing != null)
            {
               
                existing.FirstName = model.FirstName;
                existing.LastName = model.LastName;
                existing.PhoneNumber = model.PhoneNumber;

                await _context.SaveChangesAsync();
                return RedirectToAction("TeacherDashboard", "Home");
            }

            
            var teacher = new Teacher
            {
                AppUserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return RedirectToAction("TeacherDashboard", "Home");
        }
        }
}
