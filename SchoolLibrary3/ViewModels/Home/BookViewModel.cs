using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolLibrary3.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolLibrary3.ViewModels.Home
{
    public class BookViewModel: Book
    {
        [NotMapped]
        public SelectList Geners { get; set; }
        [NotMapped]
        public SelectList Statuses { get; set; }
    }
}
