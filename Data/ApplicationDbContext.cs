using Cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Models.Cinema> Cinemas { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieSubImage> MovieSubImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;initial catalog = CinemaDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        }
    }
}
