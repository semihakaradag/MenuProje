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
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .Property(c => c.Price)
                .HasPrecision(10, 2);
        }
    }
}
