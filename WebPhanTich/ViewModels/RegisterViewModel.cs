using System.ComponentModel.DataAnnotations;

namespace WebPhanTich.ViewModels
{
    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;
    }
}
