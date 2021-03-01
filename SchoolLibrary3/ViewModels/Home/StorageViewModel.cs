using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolLibrary3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolLibrary3.ViewModels.Home
{
    public class StorageViewModel
    {
        [NotMapped]
        public SelectList Books { get; set; }
    }
}
