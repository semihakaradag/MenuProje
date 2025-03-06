using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MenuProject.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using MenuProject.Data;
using Microsoft.EntityFrameworkCore;
using MenuProject.Models;

namespace MenuProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MenuDbContext _context;

        public HomeController(UserManager<IdentityUser> userManager,MenuDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Admin Ana Sayfası
        public async Task<IActionResult> Index()
        {
            var menus = await _context.UserMenus.OrderBy(m => m.SortNumber).ToListAsync();
            return View(menus);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Menus = _context.UserMenus.Where(m => m.ParentId == null).ToList(); // Ana menüleri getir
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserMenu menu)
        {
            if (!ModelState.IsValid)
            {
                _context.UserMenus.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Menus = _context.UserMenus.Where(m => m.ParentId == null).ToList();
            return View(menu);
        }

        // **📌 Menü Güncelleme Sayfası (GET)**
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var menu = await _context.UserMenus.FindAsync(id);
            if (menu == null)
                return NotFound();

            ViewBag.Menus = _context.UserMenus.Where(m => m.ParentId == null).ToList();
            return View(menu);
        }

        // **📌 Menü Güncelleme İşlemi (POST)**
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserMenu menu)
        {
            if (id != menu.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Menus = _context.UserMenus.Where(m => m.ParentId == null).ToList();
            return View(menu);
        }

        // **📌 Menü Silme İşlemi**
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.UserMenus.FindAsync(id);
            if (menu == null)
                return NotFound();

            _context.UserMenus.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }

    //// Üye Listesi Sayfası
    //public async Task<IActionResult> UserList()
    //{
    //    var users = _userManager.Users.ToList();
    //    var userRoles = new List<UserRoleViewModel>();

    //    foreach (var user in users)
    //    {
    //        var roles = await _userManager.GetRolesAsync(user);
    //        userRoles.Add(new UserRoleViewModel
    //        {
    //            Id = user.Id,
    //            UserName = user.UserName,
    //            Email = user.Email,
    //            Roles = roles.ToList()
    //        });
    //    }

    //    return View(userRoles);
    //}
}

