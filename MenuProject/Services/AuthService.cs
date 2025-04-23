using Microsoft.AspNetCore.Identity;
using MenuProject.ViewModels;
using TS.Result;

namespace MenuProject.Services
{
    public class AuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result<IdentityUser>> SignInAsync(SignInViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Result<IdentityUser>.Failure("Email veya şifre yanlış.");

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

            if (result.Succeeded)
                return Result<IdentityUser>.Succeed(user);

            if (result.IsLockedOut)
                return Result<IdentityUser>.Failure("Çok fazla giriş hatası. Lütfen bekleyin.");

            return Result<IdentityUser>.Failure("Email veya şifre yanlış.");
        }
    }
}
