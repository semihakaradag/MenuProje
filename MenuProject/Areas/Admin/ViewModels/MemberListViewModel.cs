using MenuProject.Models;

namespace MenuProject.Areas.Admin.ViewModels
{
    public class MemberListViewModel
    {
        public List<Student> Students { get; set; } = new();
        public List<Student> IncompleteStudents { get; set; } = new();

        public List<Teacher> Teachers { get; set; } = new();
        public List<Teacher> IncompleteTeachers { get; set; } = new();
    }
}
