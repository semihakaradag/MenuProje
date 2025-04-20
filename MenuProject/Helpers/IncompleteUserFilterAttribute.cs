using MenuProject.Areas.Admin.ViewModels;
using MenuProject.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MenuProject.Helpers
{
    public class IncompleteUserFilterAttribute: ActionFilterAttribute
    {
        private readonly MenuDbContext _context;

        public IncompleteUserFilterAttribute(MenuDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var model = context.ActionArguments.Values
                .FirstOrDefault(v => v is MemberListViewModel) as MemberListViewModel;

            if (model != null)
            {
                model.IncompleteStudents = model.Students
                    .Where(s => string.IsNullOrEmpty(s.PhoneNumber)).ToList();

                model.IncompleteTeachers = model.Teachers
                    .Where(t => string.IsNullOrEmpty(t.PhoneNumber)).ToList();
            }

            base.OnActionExecuting(context);
        }
    }
}
