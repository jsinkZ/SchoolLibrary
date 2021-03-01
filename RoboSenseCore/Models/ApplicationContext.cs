using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoboSenseCore.Models.Entities;

namespace RoboSenseCore.Models
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Class> Classes { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<ApplicationUser> ShUsers { get; set; }
        public DbSet<ViewModels.AccountsViewModel> AccountsViewModel { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TeamChallengers>()
            //.HasKey(t => new { t.TeamId, t.ChallengerId });

            //modelBuilder.Entity<TeamChallengers>()
            //    .HasOne(sc => sc.Team)
            //    .WithMany(c => c.TeamChallengers)
            //    .HasForeignKey(sc => sc.TeamId);

            //modelBuilder.Entity<TeamChallengers>()
            //    .HasOne(sc => sc.Challenger)
            //    .WithMany(s => s.TeamChallengers)
            //    .HasForeignKey(sc => sc.ChallengerId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
