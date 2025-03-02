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
    }
}
