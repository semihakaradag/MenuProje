using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MenuProject.ViewModels;
using System.Threading.Tasks;

namespace MenuProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetUserRole()
        //Kullanıcının giriş yapıp yapmadığını kontrol eder.
        //Eğer giriş yapmışsa kullanıcının rolünü JSON olarak döndürür.
        //Eğer giriş yapmamışsa "Guest" rolü döndürür.
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return Json(new { role = roles.Count > 0 ? roles[0] : "None" });
            }
            return Json(new { role = "Guest" });
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

            if (result.Succeeded)
            {
                // Kullanıcının rollerini al
                var roles = await _userManager.GetRolesAsync(user);

                if (model.Email == "admin@menuproject.com") // Sadece belirli admin maili ile giriş
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" }); // Admin paneline yönlendir
                }
                else if (roles.Contains("Student"))
                {
                    return RedirectToAction("StudentDashboard", "Home"); // Öğrenci sayfasına yönlendir
                }
                else if (roles.Contains("Teacher"))
                {
                    return RedirectToAction("TeacherDashboard", "Home"); // Öğretmen sayfasına yönlendir
                }

                return RedirectToAction("Index", "Home"); // Eğer rol yoksa anasayfaya yönlendir
            }


            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Çok fazla başarısız giriş yaptınız, 3 dakika sonra tekrar deneyin.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email veya şifre yanlış.");
            return View(model);
        }



        // **Kayıt Ol Sayfası**
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Form geçerli değil! Tüm alanları doğru doldurduğundan emin ol.");
                return View("SignUp", model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Student"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Student"));
                }

                await _userManager.AddToRoleAsync(user, "Student");

                TempData["SuccessMessage"] = "Kayıt başarılı! Lütfen giriş yapınız.";
                return RedirectToAction("SignIn", "Account");
            }

            // **Hata mesajlarını ekrana yazdır**
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", $"Hata: {error.Description}");
            }

            return View("SignUp", model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterTeacher(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Form geçerli değil! Tüm alanları doğru doldurduğundan emin ol.");
                return View("SignUp", model);
            }

            if (!model.Email.EndsWith("@edu.tr"))
            {
                ModelState.AddModelError("Email", "Öğretmen kaydı için e-posta @edu.tr uzantılı olmalıdır.");
                return View("SignUp", model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Teacher"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Teacher"));
                }

                await _userManager.AddToRoleAsync(user, "Teacher");

                TempData["SuccessMessage"] = "Kayıt başarılı! Lütfen giriş yapınız.";
                return RedirectToAction("SignIn", "Account");
            }

            // **Hata mesajlarını ekrana yazdır**
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", $"Hata: {error.Description}");
            }

            return View("SignUp", model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Kullanıcının oturumunu kapat
            return RedirectToAction("SignIn", "Account"); // Giriş sayfasına yönlendir
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
