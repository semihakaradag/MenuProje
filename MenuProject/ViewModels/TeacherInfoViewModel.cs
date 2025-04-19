using System.ComponentModel.DataAnnotations;

namespace MenuProject.ViewModels
{
    public class TeacherInfoViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon Numarası")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "E-Posta")]
        [EmailAddress]
        public string Email { get; set; } // readonly olarak formda gösterilecek
    }
}
