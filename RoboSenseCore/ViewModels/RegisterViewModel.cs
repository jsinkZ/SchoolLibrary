using System;
using System.ComponentModel.DataAnnotations;

namespace RoboSenseCore.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле «Email» должно быть заполнено")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле «Представительство» должно быть заполнено")]
        [Display(Name = "Представительство")]
        public Guid Deputizing { get; set; }

        [Display(Name = "Свободное представительство")]
        public Boolean FreeDeputizing { get; set; }

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
