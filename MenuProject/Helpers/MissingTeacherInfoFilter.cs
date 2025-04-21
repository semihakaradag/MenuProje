using MenuProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MenuProject.Helpers
{
    public class MissingTeacherInfoFilter : IAsyncActionFilter
    {
        private readonly MenuDbContext _context;
        public MissingTeacherInfoFilter(MenuDbContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                await next();
                return;
            }

            var userId = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (userId == null)
            {
                await next();
                return;
            }

            // Öğretmense ve bilgileri eksikse yönlendir
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.AppUserId == userId);

            if (teacher == null ||
                string.IsNullOrWhiteSpace(teacher.FirstName) ||
                string.IsNullOrWhiteSpace(teacher.LastName) ||
                string.IsNullOrWhiteSpace(teacher.PhoneNumber))
            {
                context.Result = new RedirectToActionResult("Info", "Teacher", null);
                return;
            }

            await next();
        }
    }
}