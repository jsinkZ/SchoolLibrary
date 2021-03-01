using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RoboSenseCore.Models
{
    public class Book
    {
        [Key]    
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public Guid GenreId { get; set; }
        public int TotalPages { get; set; }

    }
}
