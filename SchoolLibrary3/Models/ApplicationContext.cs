using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolLibrary3.Models.Entities;

namespace SchoolLibrary3.Models
{
    public class ApplicationContext : IdentityDbContext<User> //DbContext 
    {

        // Вспомогательные наборы для прямых запросов SQL - начало
        //public DbSet<ViewModels.AccountsViewModel> AccountsViewModel { get; set; }
        public DbSet<User> User { get; set; }

        // Вспомогательные наборы для прямых запросов SQL - конец
        public DbSet<Book> Books { get; set; }
        public DbSet<BookStatus> Statuses { get; set; }
        public DbSet<Genre> Genres { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }   
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

    }
}
