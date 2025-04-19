using System.ComponentModel.DataAnnotations;

namespace MenuProject.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Ders adı zorunludur.")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name = "Minimum Doğum Yılı")]
        public int? MinBirthYear { get; set; }

        [Display(Name = "Maksimum Doğum Yılı")]
        public int? MaxBirthYear { get; set; }

        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Eğitmen Adı")]
        public string TeacherName { get; set; }

        [Display(Name = "Fiyat")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat negatif olamaz.")]
        public decimal Price { get; set; }
    }
}
