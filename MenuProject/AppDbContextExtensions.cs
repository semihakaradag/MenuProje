using MenuProject.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using MenuProject.Data;

public static class AppDbContextExtensions
{
    public static void SeedData(IServiceProvider serviceProvider)
    {
        using (var context = serviceProvider.GetRequiredService<MenuDbContext>())
        {
            // Eğer UserMenus boşsa menüleri ekleyelim
            if (!context.UserMenus.Any())
            {
                var dashboardMenu = new UserMenu
                {
                    Name = "Anasayfa",
                    ControllerName = "Home",
                    ActionName = "Dashboard",
                    Icon = "fa-solid fa-home",
                    SortNumber = 1
                };

                var studentMenu = new UserMenu
                {
                    Name = "Derslerim",
                    ControllerName = "Student",
                    ActionName = "Courses",
                    Icon = "fa-solid fa-book",
                    SortNumber = 2
                };

                var teacherMenu = new UserMenu
                {
                    Name = "Derslerim",
                    ControllerName = "Teacher",
                    ActionName = "MyCourses",
                    Icon = "fa-solid fa-chalkboard-teacher",
                    SortNumber = 3
                };

                var adminMenu = new UserMenu
                {
                    Name = "Form İşlemleri",
                    ControllerName = "Admin",
                    ActionName = "FormList",
                    Icon = "fa-solid fa-list",
                    SortNumber = 4
                };

                context.UserMenus.AddRange(dashboardMenu, studentMenu, teacherMenu, adminMenu);
                context.SaveChanges();

                // Şimdi eklenen menülerin ID'lerini alalım
                var dashboardId = dashboardMenu.Id;
                var studentMenuId = studentMenu.Id;
                var teacherMenuId = teacherMenu.Id;
                var adminMenuId = adminMenu.Id;

                // Eğer RoleMenus boşsa rolleri ekleyelim
                if (!context.RoleMenus.Any())
                {
                    context.RoleMenus.AddRange(
                        new RoleMenu { RoleName = "Admin", MenuId = dashboardId },
                        new RoleMenu { RoleName = "Admin", MenuId = adminMenuId },

                        new RoleMenu { RoleName = "Student", MenuId = dashboardId },
                        new RoleMenu { RoleName = "Student", MenuId = studentMenuId },

                        new RoleMenu { RoleName = "Teacher", MenuId = dashboardId },
                        new RoleMenu { RoleName = "Teacher", MenuId = teacherMenuId }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}
