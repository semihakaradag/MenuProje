using MenuProject.Models;
using System.Collections.Generic;


namespace MenuProject.ViewModels
{
    public class UserRoleViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        // Öğrenci ve öğretmen sayıları
        public int StudentCount { get; set; }
        public int TeacherCount { get; set; }

        // Menü Listesi
        public List<UserMenu> Menus { get; set; }
    }
}
