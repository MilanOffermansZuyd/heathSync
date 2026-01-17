using HealthSync.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthSync.Data
{
    public class HealthSyncContext : DbContext
    {
        public DbSet<User> Gebruikers { get; set; }

        public HealthSyncContext(DbContextOptions<HealthSyncContext> option) : base(option)
        {
            Database.EnsureDeleted(); // Tijdelijke fiks voor de database opnieuw te herstellen voor zowel Windows als Android zodat ik niet elke folder af hoef te zoeken
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Naam = "Hans" }
                );
        }
    }
}