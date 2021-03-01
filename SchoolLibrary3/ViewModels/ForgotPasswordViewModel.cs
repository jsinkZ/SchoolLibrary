using System.ComponentModel.DataAnnotations;

namespace SchoolLibrary3.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле «Email» должно быть заполнено")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
