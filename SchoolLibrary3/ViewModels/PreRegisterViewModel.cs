using Microsoft.AspNetCore.Identity;
using SchoolLibrary3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolLibrary3.ViewModels
{
    public class PreRegisterViewModel
    {
        [Required(ErrorMessage = "Поле «Email» должно быть заполнено")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [NotMapped]
        public String RoleName { get; set; }
        [NotMapped]
        public List<IdentityRole> Roles { get; set; }
    }
}
