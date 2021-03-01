using System;

namespace SchoolLibrary3.Models.Entities
{
    public class Book 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public Guid GenreId { get; set; }
        public int TotalPages { get; set; }
        public String Publisher { get; set; }
        public Guid StatusId { get; set; }
    }
}
