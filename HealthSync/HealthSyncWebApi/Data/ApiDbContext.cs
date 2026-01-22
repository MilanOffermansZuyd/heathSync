using HealthSyncWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthSyncWebApi.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<PrescriptionRequest> PrescriptionRequests { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PrescriptionRequest>()
                .HasIndex(r => r.ClientRequestId)
                .IsUnique();

            modelBuilder.Entity<PrescriptionRequest>()
                .HasOne(r => r.ApprovedPrescription)
                .WithMany()
                .HasForeignKey(r => r.ApprovedPrescriptionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
