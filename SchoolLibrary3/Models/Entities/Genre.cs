using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolLibrary3.Models.Entities
{
    public class Genre
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
    }
}
