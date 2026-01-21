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
            Database.EnsureDeleted(); // Tijdelijke fiks voor de database opnieuw te herstellen voor zowel Windows als Android zodat ik niet elke folder af hoef te zoeken
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName="Admin", LastName="Administrator", Email="admin@admin.nl", Password="admin", Role= UserRole.Patient},
                new User { Id = 2, FirstName = "Frits", LastName = "Spits", Email = "f.spits@spits.nl", Password = "fspits", Role = UserRole.Customer },
                new User { Id = 3, FirstName = "Miep", LastName = "Bliep", Email = "m.bliep@bliep.nl", Password = "mbliep", Role = UserRole.Customer },
                new User { Id = 4, FirstName = "Ronald", LastName = "Wemel", Email = "r.wemel@wemel.nl", Password = "rwemel", Role = UserRole.Patient }
                );
        }
    }
}