using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MenuProject.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using MenuProject.Data;
using Microsoft.EntityFrameworkCore;
using MenuProject.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Internal;
using MenuProject.Areas.Admin.ViewModels;

namespace MenuProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MenuDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private object _contextFactory;
        private object _logger;

        public AdminController(UserManager<IdentityUser> userManager,MenuDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        // Admin Ana Sayfası
        public async Task<IActionResult> Index()
        {
            // Kullanıcıları getir
            var users = await _userManager.Users.ToListAsync();

            // Rollerine göre kullanıcıları filtrele
            var studentCount = 0;
            var teacherCount = 0;

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Student"))
                {
                    studentCount++;
                }
                else if (roles.Contains("Teacher"))
                {
                    teacherCount++;
                }
            }

            // Veritabanından menüleri çek
            var menus = await _context.UserMenus.OrderBy(m => m.SortNumber).ToListAsync();

            // Modeli oluştur
            var model = new UserRoleViewModel
            {
                StudentCount = studentCount,
                TeacherCount = teacherCount,
                Menus = menus // Menüleri modele ekledik
            };

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
                // 1️⃣ Yeni menünün SortNumber'ını belirle
                int newSortNumber = 1; // Varsayılan değer

                if (menu.ParentId == null)
                {
                    // Ana menü ise en büyük ana menünün sırasını bulup +1 ekle
                    var maxSort = _context.UserMenus.Where(m => m.ParentId == null).Max(m => (int?)m.SortNumber) ?? 0;
                    newSortNumber = maxSort + 1;
                }
                else
                {
                    // Alt menü ise aynı ParentId'ye sahip en büyük SortNumber'ı bul
                    var maxSort = _context.UserMenus.Where(m => m.ParentId == menu.ParentId).Max(m => (int?)m.SortNumber) ?? 0;
                    newSortNumber = maxSort + 1;
                }

                menu.SortNumber = newSortNumber;

                // 2️⃣ UserMenus tablosuna menüyü ekle
                _context.UserMenus.Add(menu);
                _context.SaveChanges();

                // 3️⃣ Seçilen rol varsa, RoleMenus tablosuna da ekle
                if (!string.IsNullOrEmpty(menu.SelectedRole))
                {
                    var roleMenu = new RoleMenu
                    {
                        RoleName = menu.SelectedRole, // Seçilen rolü al
                        MenuId = menu.Id // Eklenen menünün ID'si
                    };

                    _context.RoleMenus.Add(roleMenu);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View(menu);
        }

        // **📌 Menü Güncelleme Sayfası (GET)**
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var menu = await _context.UserMenus.FindAsync(id);
            if (menu == null)
                return NotFound();

            // **📌 Doğru tabloya erişerek rollerin adlarını al**
            ViewBag.Roles = await _context.Set<IdentityRole>().Select(r => r.Name).ToListAsync() ?? new List<string>();

            ViewBag.Menus = await _context.UserMenus.Where(m => m.ParentId == null).ToListAsync();
            return View(menu);
        }


        // **📌 Menü Güncelleme İşlemi (POST)**
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserMenu menu)
        {
            if (id != menu.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                try
                {
                    var existingMenu = await _context.UserMenus.FindAsync(id);
                    if (existingMenu != null)
                    {
                        existingMenu.Name = menu.Name;
                        existingMenu.ControllerName = menu.ControllerName;
                        existingMenu.ActionName = menu.ActionName;
                        existingMenu.SortNumber = menu.SortNumber;
                        existingMenu.ParentId = menu.ParentId;
                        existingMenu.IsVisible = true;

                        _context.Update(existingMenu);
                        await _context.SaveChangesAsync();

                        var user = await _userManager.GetUserAsync(User);
                        if (user != null)
                        {
                            var claimsService = new ClaimsService(_userManager, _contextFactory, _logger);
                            await claimsService.UpdateUserClaims(user);
                            await _signInManager.RefreshSignInAsync(user);
                        }
                    }

                    // **📌 JSON yerine doğrudan Dashboard sayfasına yönlendiriyoruz!**
                    return RedirectToAction("Index", new { area = "Admin" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Güncelleme hatası: " + ex.Message);
                }
            }

            ViewBag.Roles = await _context.Set<IdentityRole>().Select(r => r.Name).ToListAsync() ?? new List<string>();
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

