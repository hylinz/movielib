using MovieLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace MovieLibrary
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().Property(property => property.Name).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(property => property.Name).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(property => property.Picture).IsUnicode();

        }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
    }
}
