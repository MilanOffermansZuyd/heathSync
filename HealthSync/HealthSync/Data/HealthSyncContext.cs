using HealthSync.Models;
using HealthSync.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HealthSync.Data
{
    public class HealthSyncContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        //public DbSet<Customer> Customers { get; set; }
        //public DbSet<Patient> Patients { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }

        public DbSet<Medication> Medications { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionRequest> PrescriptionRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<HealthData> HealthDatas { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }

        public HealthSyncContext(DbContextOptions<HealthSyncContext> option) : base(option)
        {
            Database.EnsureDeleted(); // Tijdelijke fiks voor de database opnieuw te herstellen voor zowel Windows als Android zodat ik niet elke folder af hoef te zoeken
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User -> HealthData
            modelBuilder.Entity<User>()
                .HasOne(u => u.HealthData)
                .WithOne(h => h.User)
                .HasForeignKey<HealthData>(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> EmergencyContacts
            modelBuilder.Entity<User>()
                .HasMany(u => u.EmergencyContacts)
                .WithOne()
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor -> Address
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Address)
                .WithMany()
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pharmacy -> Address
            modelBuilder.Entity<Pharmacy>()
                .HasOne(p => p.Address)
                .WithMany()
                .HasForeignKey(p => p.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Prescriptions
            modelBuilder.Entity<User>()
                .HasMany(u => u.Prescriptions)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> PrescriptionRequests
            modelBuilder.Entity<User>()
                .HasMany(u => u.PrescriptionRequests)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PrescriptionRequest -> Medication
            modelBuilder.Entity<PrescriptionRequest>()
                .HasOne(r => r.Medication)
                .WithMany()
                .HasForeignKey(r => r.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // PrescriptionRequest -> Pharmacy
            modelBuilder.Entity<PrescriptionRequest>()
                .HasOne(r => r.Pharmacy)
                .WithMany()
                .HasForeignKey(r => r.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);

            // PrescriptionRequest -> Doctor
            modelBuilder.Entity<PrescriptionRequest>()
                .HasOne(r => r.Doctor)
                .WithMany()
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // PrescriptionRequest -> ApprovedPrescription (optioneel, 0..1)
            modelBuilder.Entity<PrescriptionRequest>()
                .HasOne(r => r.ApprovedPrescription)
                .WithMany()
                .HasForeignKey(r => r.ApprovedPrescriptionId)
                .OnDelete(DeleteBehavior.SetNull);

            // ClientRequestId uniek maken in je lokale DB EVEN IN DE GATEN HOUDEN
            modelBuilder.Entity<PrescriptionRequest>()
                .HasIndex(r => r.ClientRequestId)
                .IsUnique();

            // Prescription -> Medication
            modelBuilder.Entity<Prescription>()
                .HasOne(pr => pr.Medication)
                .WithMany()
                .HasForeignKey(pr => pr.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prescription -> Pharmacy
            modelBuilder.Entity<Prescription>()
                .HasOne(pr => pr.Pharmacy)
                .WithMany()
                .HasForeignKey(pr => pr.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prescription -> Doctor
            modelBuilder.Entity<Prescription>()
                .HasOne(pr => pr.Doctor)
                .WithMany()
                .HasForeignKey(pr => pr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification -> Prescription
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Prescription)
                .WithMany()
                .HasForeignKey(n => n.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // User
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName= "Admin", LastName= "Administrator", Email= "admin@admin.nl", Password= "admin", Role= UserRole.Patient, NotificationsEnabled = true},
                new User { Id = 2, FirstName = "Frits", LastName = "Spits", Email = "f.spits@spits.nl", Password = "fspits", Role = UserRole.Customer, NotificationsEnabled = true },
                new User { Id = 3, FirstName = "Miep", LastName = "Bliep", Email = "m.bliep@bliep.nl", Password = "mbliep", Role = UserRole.Customer, NotificationsEnabled = true },
                new User { Id = 4, FirstName = "Ronald", LastName = "Wemel", Email = "r.wemel@wemel.nl", Password = "rwemel", Role = UserRole.Patient, NotificationsEnabled = true }
                );

            // Address
            modelBuilder.Entity<Address>().HasData(
                new Address { Id = 1, Street = "Stationsstraat 1", PostalCode = "1234 AB", Place = "Maastricht", Country = "NL" },
                new Address { Id = 2, Street = "Dorpsplein 10", PostalCode = "5678 CD", Place = "Heerlen", Country = "NL" }
                );

            // Doctor
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FirstName = "Henk", LastName = "Jansen", Clinic = "Huisartsenpraktijk Centrum", AddressId = 1 }
                );

            // Pharmacy
            modelBuilder.Entity<Pharmacy>().HasData(
                new Pharmacy { Id = 1, PharmacyName = "Apotheek Zuid", EmployeeId = 1001, AddressId = 2 }
                );

            // Medication
            modelBuilder.Entity<Medication>().HasData(
                new Medication { Id = 1, Name = "Paracetamol", Strength = "500mg", Form = "Tablet" },
                new Medication { Id = 2, Name = "Ibuprofen", Strength = "400mg", Form = "Tablet" }
            );

            // Prescription
            modelBuilder.Entity<Prescription>().HasData(
                new Prescription {
                    Id = 1,
                    UserId = 1,
                    MedicationId = 1,
                    PharmacyId = 1,
                    DoctorId = 1,
                    DateOfPrescription = new DateTime(2026, 1, 1),
                    Amount = 30,
                    Unit = "stuks",
                    RefillsRemaining = 1,
                    TakeCount = 3,
                    TimeBetweenMinutes = 360,
                    Instruction = "Na het eten innemen"
                },
                new Prescription
                {
                    Id = 2,
                    UserId = 4,
                    MedicationId = 2,
                    PharmacyId = 1,
                    DoctorId = 1,
                    DateOfPrescription = new DateTime(2026, 1, 10),
                    Amount = 20,
                    Unit = "stuks",
                    RefillsRemaining = 0,
                    TakeCount = 2,
                    TimeBetweenMinutes = 480,
                    Instruction = "Met water innemen"
                }
            );

            // Notification
            modelBuilder.Entity<Notification>().HasData(
                new Notification { Id = 1, PrescriptionId = 1, Message = "Tijd voor medicatie (Paracetamol).", IsSent = false },
                new Notification { Id = 2, PrescriptionId = 2, Message = "Tijd voor medicatie (Ibuprofen).", IsSent = false }
            );

            // HealthData
            modelBuilder.Entity<HealthData>().HasData(
                new HealthData
                {
                    Id = 1,
                    UserId = 1,
                    HeartRateInRest = 62,
                    StartSleep = new DateTime(2026, 1, 20, 23, 0, 0),
                    EndSleep = new DateTime(2026, 1, 21, 7, 0, 0),
                    MinutesAwake = 20,
                    StepCount = 5400
                },
                new HealthData
                {
                    Id = 2,
                    UserId = 4,
                    HeartRateInRest = 68,
                    StartSleep = new DateTime(2026, 1, 20, 22, 30, 0),
                    EndSleep = new DateTime(2026, 1, 21, 6, 30, 0),
                    MinutesAwake = 35,
                    StepCount = 7200
                }
            );
        }
    }
}