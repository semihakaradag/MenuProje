using MenuProject.Models;

namespace MenuProject.Areas.Admin.ViewModels
{
    public class MemberListViewModel
    {
        public List<Student> Students { get; set; }
        public List<Student> IncompleteStudents { get; set; }

        public List<Teacher> Teachers { get; set; }
        public List<Teacher> IncompleteTeachers { get; set; }
    }
}
