using MenuProject.Data;
using MenuProject.Models;

namespace MenuProject.Services
{
    public class CourseService : ICourseService
    {
        private readonly MenuDbContext _context;

        public CourseService(MenuDbContext context)
        {
            _context = context;
        }

        public void AddCourse(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        public List<Course> GetAllCourses()
        {
            return _context.Courses.ToList();
        }
    }
}
