using Microsoft.AspNetCore.Mvc;

namespace MenuProject.Areas.Admin
{
    public class AdminAreaRegistration : AreaAttribute
    {
        public AdminAreaRegistration() : base("Admin")
        {
        }
    }
}
