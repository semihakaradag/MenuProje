using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MenuProject.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MenuProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ClaimsService _claimsService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ClaimsService claimsService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _claimsService = claimsService;
            _logger = logger;
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetUserClaims()
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Json(userClaims);
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
                var claims = await _claimsService.GetUserClaims(user);
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                //  Kullanıcının rollerini alıp claim olarak ekleyelim
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));  // Rol claim olarak ekleniyor
                }

                if (claims.Any())
                {
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Önce eski oturumu temizleyelim
                    await _signInManager.SignOutAsync();
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    // Yeni claim'lerle oturumu başlatalım
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddHours(1)
                        });

                    _logger.LogInformation("Kullanıcı için menü claimleri ve rol claimleri eklendi.");
                }
                else
                {
                    _logger.LogError("Kullanıcı için herhangi bir menü veya rol claimi bulunamadı.");
                }


                // Rol bazlı yönlendirme burada!
                if (userRoles.Contains("Admin"))
                    return RedirectToAction("AdminDashboard", "Home", new { area = "Admin" });
                else if (userRoles.Contains("Student"))
                    return RedirectToAction("StudentDashboard", "Home");
                else if (userRoles.Contains("Teacher"))
                    return RedirectToAction("TeacherDashboard", "Home");

                return RedirectToAction("Index", "Home"); // fallback (rol yoksa)
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Çok fazla başarısız giriş yaptınız, 3 dakika sonra tekrar deneyin.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email veya şifre yanlış.");
            return View(model);
        }
        [HttpGet]
        public IActionResult DebugClaims()
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"DebugClaims -> Type: {claim.Type}, Value: {claim.Value}");
            }

            return Json(userClaims);
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
            await HttpContext.SignOutAsync(); // Claims'leri temizle

            return RedirectToAction("SignIn", "Account"); // Giriş sayfasına yönlendir
        }

    }
}
