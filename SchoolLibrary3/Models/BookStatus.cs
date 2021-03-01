using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolLibrary3.Models
{
    public class BookStatus
    {   
        [Key]
        public Guid bookId { get; set; }
        public string status { get; set; }
    }
}
