using MenuProject.Models;

namespace MenuProject.Services
{
    public interface ICourseService
    {
        void AddCourse(Course course);
        List<Course> GetAllCourses();
    }
}
