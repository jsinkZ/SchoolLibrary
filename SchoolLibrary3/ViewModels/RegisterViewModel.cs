using System.ComponentModel.DataAnnotations;

namespace SchoolLibrary3.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле «Email» должно быть заполнено")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле «Имя» обязательно для ввода")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле «Фамилия» обязательно для ввода")]
        [Display(Name = "Фамилия")]
        public string SecondName { get; set; }

        [Required(ErrorMessage = "Поле «Отчество» обязательно для ввода")]
        [Display(Name = "Отчество")]
        public string PatronymicName { get; set; }

        [Required(ErrorMessage = "Поле «Пароль» обязательно для ввода")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Поле «Подтвердить пароль» обязательно для ввода")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }

        public string TheToken { get; set; }
    }
}
