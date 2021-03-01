using Microsoft.AspNetCore.Mvc.Rendering;
using RoboSenseCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboSenseCore.ViewModels
{
    public class PreRegisterViewModel
    {
        [Required(ErrorMessage = "Поле «Email» должно быть заполнено")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Представительство")]
        public Guid Deputizing { get; set; }

        [Required]
        [Display(Name = "Свободное представительство")]
        public Boolean FreeDeputizing { get; set; }

        [NotMapped]
        public SelectList Cities { get; set; }

        [NotMapped]
        public Guid CityId { get; set; }

        [NotMapped]
        public List<Class> Schools { get; set; }
    }
}
