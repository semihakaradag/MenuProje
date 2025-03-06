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

    public ClaimsService(UserManager<IdentityUser> userManager, IDbContextFactory<MenuDbContext> contextFactory, ILogger<ClaimsService> logger)
    {
        _userManager = userManager;
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<List<Claim>> GetUserClaims(IdentityUser user)
    {
        var claims = new List<Claim>();

        using (var context = _contextFactory.CreateDbContext())
        {
            // Kullanıcının rollerini al
            var userRoles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("Kullanıcının Rolleri: {Roles}", string.Join(", ", userRoles));

            if (!userRoles.Any()) return claims;

            // RoleMenus'dan menü ID'lerini al
            var roleMenuIds = await context.RoleMenus
                .Where(rm => userRoles.Contains(rm.RoleName))
                .Select(rm => rm.MenuId)
                .ToListAsync();

            _logger.LogInformation("Erişilebilir RoleMenus: {Count} adet bulundu.", roleMenuIds.Count);

            if (!roleMenuIds.Any()) return claims;

            // Yeni bir DbContext oluştur ve UserMenus'dan verileri al
            using (var newContext = _contextFactory.CreateDbContext())
            {
                var userMenus = await newContext.UserMenus
                    .Where(m => roleMenuIds.Contains(m.Id))
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Erişilebilir UserMenus: {Count} adet bulundu.", userMenus.Count);

                // Menüleri claim olarak ekleyelim
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
}
