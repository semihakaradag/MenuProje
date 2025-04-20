using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MenuProject.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public IdentityUser AppUser { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        // Otomatik yaş hesabı için (veritabanına kaydedilmez)
        [NotMapped]
        public int Age => DateTime.Now.Year - BirthDate.Year;
    }
}
