using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MenuProject.Data;

public class ClaimsService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IDbContextFactory<MenuDbContext> _contextFactory;
    private readonly ILogger<ClaimsService> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private object contextFactory;
    private object logger;

    public ClaimsService(UserManager<IdentityUser> userManager, object contextFactory, object logger)
    {
        _userManager = userManager;
        this.contextFactory = contextFactory;
        this.logger = logger;
    }

    public ClaimsService(UserManager<IdentityUser> userManager, IDbContextFactory<MenuDbContext> contextFactory, ILogger<ClaimsService> logger, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _contextFactory = contextFactory;
        _logger = logger;
        _signInManager = signInManager;
    }
   

    public async Task<List<Claim>> GetUserClaims(IdentityUser user)
    {
        var claims = new List<Claim>();

        using (var context = _contextFactory.CreateDbContext())
        {
            
            var userRoles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("Kullanıcının Rolleri: {Roles}", string.Join(", ", userRoles));

            if (!userRoles.Any()) return claims;

          
            var roleMenuIds = await context.RoleMenus
                .Where(rm => userRoles.Contains(rm.RoleName))
                .Select(rm => rm.MenuId)
                .ToListAsync();

            _logger.LogInformation("Erişilebilir RoleMenus: {Count} adet bulundu.", roleMenuIds.Count);

            if (!roleMenuIds.Any()) return claims;
 
            using (var newContext = _contextFactory.CreateDbContext())
            {
                var userMenus = await newContext.UserMenus
                    .Where(m => roleMenuIds.Contains(m.Id))
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Erişilebilir UserMenus: {Count} adet bulundu.", userMenus.Count);

             
                foreach (var menu in userMenus)
                {
                    var claimValue = $"{menu.ControllerName}/{menu.ActionName}/{menu.Name}";
                    _logger.LogInformation("Eklenen Menü Claim: {Claim}", claimValue);
                    claims.Add(new Claim("Menu", claimValue));
                }
            }
        }

        return claims;
    }
    //u metod, kullanıcının claim'lerini sıfırlayıp güncelleyerek yetkilerini yeniliyor.
    public async Task UpdateUserClaims(IdentityUser user)
    {
        if (user == null)
            return;

        using (var context = _contextFactory.CreateDbContext())
        {
            // Kullanıcının mevcut claim'lerini al
            var existingClaims = await _userManager.GetClaimsAsync(user);

            // Eski menü claim'lerini temizle
            var menuClaims = existingClaims.Where(c => c.Type == "Menu").ToList();
            foreach (var claim in menuClaims)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }

            // Kullanıcının rollerini al
            var userRoles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("Kullanıcının Güncellenmiş Rolleri: {Roles}", string.Join(", ", userRoles));

            if (!userRoles.Any()) return;

            // RoleMenus'dan menü ID'lerini al
            var roleMenuIds = await context.RoleMenus
                .Where(rm => userRoles.Contains(rm.RoleName))
                .Select(rm => rm.MenuId)
                .ToListAsync();

            _logger.LogInformation("Güncellenmiş RoleMenus: {Count} adet bulundu.", roleMenuIds.Count);

            if (!roleMenuIds.Any()) return;

            // Güncellenmiş menüleri UserMenus tablosundan çek
            var updatedMenus = await context.UserMenus
                .Where(m => roleMenuIds.Contains(m.Id))
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Güncellenmiş UserMenus: {Count} adet bulundu.", updatedMenus.Count);

            // Yeni menüleri claim olarak ekleyelim
            foreach (var menu in updatedMenus)
            {
                var claimValue = $"{menu.ControllerName}/{menu.ActionName}/{menu.Name}";
                _logger.LogInformation("Yeni Eklenen Menü Claim: {Claim}", claimValue);
                await _userManager.AddClaimAsync(user, new Claim("Menu", claimValue));
            }
        }

        // **📌 Kullanıcıyı yeniden oturuma alarak claim'leri güncelle**
        await _signInManager.RefreshSignInAsync(user);
    }

}
