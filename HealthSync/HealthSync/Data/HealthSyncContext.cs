using HealthSync.Models;
using HealthSync.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HealthSync.Data
{
    public class HealthSyncContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public HealthSyncContext(DbContextOptions<HealthSyncContext> option) : base(option)
        {
            Database.EnsureDeleted(); //  TEST Tijdelijke fiks voor de database opnieuw te herstellen voor zowel Windows als Android zodat ik niet elke folder af hoef te zoeken
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Voornaam="Admin", Achternaam="Administrator", Email="admin@admin.nl", Wachtwoord="admin", Role= UserRole.Patient},
                new User { Id = 2, Voornaam = "Frits", Achternaam = "Spits", Email = "f.spits@spits.nl", Wachtwoord = "fspits", Role = UserRole.Customer },
                new User { Id = 3, Voornaam = "Miep", Achternaam = "Bliep", Email = "m.bliep@bliep.nl", Wachtwoord = "mbliep", Role = UserRole.Customer },
                new User { Id = 4, Voornaam = "Ronald", Achternaam = "Wemel", Email = "r.wemel@wemel.nl", Wachtwoord = "rwemel", Role = UserRole.Patient }
                );
        }
    }
}