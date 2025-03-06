using MenuProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MenuProject.Data
{
    public class MenuDbContext : IdentityDbContext<IdentityUser>
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options)
        {

        }
        public DbSet<UserMenu> UserMenus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
    }
}
