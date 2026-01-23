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
                new Address { Id = 2, Street = "Dorpsplein 10", PostalCode = "5678 CD", Place = "Heerlen", Country = "NL" },
                new Address { Id = 3, Street = "Markt 5", PostalCode = "6211 CK", Place = "Maastricht", Country = "NL" },
                new Address { Id = 4, Street = "Kerkstraat 12", PostalCode = "6211 JP", Place = "Maastricht", Country = "NL" },
                new Address { Id = 5, Street = "Molenweg 8", PostalCode = "6411 AA", Place = "Heerlen", Country = "NL" },
                new Address { Id = 6, Street = "Julianastraat 22", PostalCode = "6411 BC", Place = "Heerlen", Country = "NL" },
                new Address { Id = 7, Street = "Rijksweg 33", PostalCode = "6161 AB", Place = "Geleen", Country = "NL" },
                new Address { Id = 8, Street = "Schoolstraat 9", PostalCode = "6131 KL", Place = "Sittard", Country = "NL" },
                new Address { Id = 9, Street = "Maasboulevard 17", PostalCode = "6211 JW", Place = "Maastricht", Country = "NL" },
                new Address { Id = 10, Street = "Hoofdstraat 101", PostalCode = "6372 AS", Place = "Landgraaf", Country = "NL" },
                new Address { Id = 11, Street = "Prinsengracht 44", PostalCode = "1015 DX", Place = "Amsterdam", Country = "NL" },
                new Address { Id = 12, Street = "Coolsingel 120", PostalCode = "3011 AD", Place = "Rotterdam", Country = "NL" },
                new Address { Id = 13, Street = "Kruisstraat 6", PostalCode = "5612 CJ", Place = "Eindhoven", Country = "NL" },
                new Address { Id = 14, Street = "Oude Gracht 88", PostalCode = "3511 AV", Place = "Utrecht", Country = "NL" },
                new Address { Id = 15, Street = "Langestraat 14", PostalCode = "4701 RD", Place = "Roosendaal", Country = "NL" },
                new Address { Id = 16, Street = "Dorpstraat 3", PostalCode = "6245 AA", Place = "Eijsden", Country = "NL" },
                new Address { Id = 17, Street = "Wilhelminaplein 2", PostalCode = "6411 KV", Place = "Heerlen", Country = "NL" },
                new Address { Id = 18, Street = "Nieuwstraat 19", PostalCode = "6211 CR", Place = "Maastricht", Country = "NL" },
                new Address { Id = 19, Street = "Burgemeesterstraat 7", PostalCode = "6361 GD", Place = "Nuth", Country = "NL" },
                new Address { Id = 20, Street = "Parklaan 55", PostalCode = "6131 BT", Place = "Sittard", Country = "NL" },
                new Address { Id = 21, Street = "Beekstraat 25", PostalCode = "6191 AB", Place = "Beek", Country = "NL" },
                new Address { Id = 22, Street = "Heuvelstraat 40", PostalCode = "5038 AE", Place = "Tilburg", Country = "NL" }
            );

            // Doctor
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FirstName = "Henk", LastName = "Jansen", Clinic = "Huisartsenpraktijk Centrum", AddressId = 1 },
                new Doctor { Id = 2, FirstName = "Sanne", LastName = "de Vries", Clinic = "Huisartsenpraktijk Markt", AddressId = 3 },
                new Doctor { Id = 3, FirstName = "Youssef", LastName = "Bakker", Clinic = "Medisch Centrum Heerlen", AddressId = 5 },
                new Doctor { Id = 4, FirstName = "Lotte", LastName = "van Dijk", Clinic = "Huisartsenpraktijk Julianastraat", AddressId = 6 },
                new Doctor { Id = 5, FirstName = "Daan", LastName = "Meijer", Clinic = "Gezondheidscentrum Geleen", AddressId = 7 },
                new Doctor { Id = 6, FirstName = "Nora", LastName = "Smit", Clinic = "Huisartsenpraktijk Sittard", AddressId = 8 },
                new Doctor { Id = 7, FirstName = "Bram", LastName = "Willems", Clinic = "Maas Kliniek", AddressId = 9 },
                new Doctor { Id = 8, FirstName = "Amina", LastName = "El Amrani", Clinic = "Huisartsenpraktijk Landgraaf", AddressId = 10 },
                new Doctor { Id = 9, FirstName = "Thomas", LastName = "Peters", Clinic = "Huisartsenpraktijk Prinsengracht", AddressId = 11 },
                new Doctor { Id = 10, FirstName = "Esmee", LastName = "Koster", Clinic = "Stadsdokters Rotterdam", AddressId = 12 },
                new Doctor { Id = 11, FirstName = "Ruben", LastName = "Mulder", Clinic = "Huisartsenpraktijk Eindhoven Centrum", AddressId = 13 }
            );

            // Pharmacy
            modelBuilder.Entity<Pharmacy>().HasData(
                new Pharmacy { Id = 1, PharmacyName = "Apotheek Zuid", EmployeeId = 1001, AddressId = 2 },
                new Pharmacy { Id = 2, PharmacyName = "Apotheek Centrum Maastricht", EmployeeId = 1002, AddressId = 4 },
                new Pharmacy { Id = 3, PharmacyName = "Apotheek Heerlen Noord", EmployeeId = 1003, AddressId = 17 },
                new Pharmacy { Id = 4, PharmacyName = "Apotheek Geleen", EmployeeId = 1004, AddressId = 7 },
                new Pharmacy { Id = 5, PharmacyName = "Apotheek Sittard Stad", EmployeeId = 1005, AddressId = 20 },
                new Pharmacy { Id = 6, PharmacyName = "Apotheek Beek", EmployeeId = 1006, AddressId = 21 },
                new Pharmacy { Id = 7, PharmacyName = "Apotheek Amsterdam Gracht", EmployeeId = 1007, AddressId = 11 },
                new Pharmacy { Id = 8, PharmacyName = "Apotheek Rotterdam Centrum", EmployeeId = 1008, AddressId = 12 },
                new Pharmacy { Id = 9, PharmacyName = "Apotheek Utrecht Gracht", EmployeeId = 1009, AddressId = 14 },
                new Pharmacy { Id = 10, PharmacyName = "Apotheek Tilburg Heuvel", EmployeeId = 1010, AddressId = 22 },
                new Pharmacy { Id = 11, PharmacyName = "Apotheek Eijsden", EmployeeId = 1011, AddressId = 16 }
            );

            // Medication
            modelBuilder.Entity<Medication>().HasData(
                new Medication { Id = 1, Name = "Paracetamol", Strength = "500mg", Form = "Tablet" },
                new Medication { Id = 2, Name = "Ibuprofen", Strength = "400mg", Form = "Tablet" },
                new Medication { Id = 3, Name = "Amoxicilline", Strength = "500mg", Form = "Capsule" },
                new Medication { Id = 4, Name = "Omeprazol", Strength = "20mg", Form = "Capsule" },
                new Medication { Id = 5, Name = "Metformine", Strength = "500mg", Form = "Tablet" },
                new Medication { Id = 6, Name = "Atorvastatine", Strength = "20mg", Form = "Tablet" },
                new Medication { Id = 7, Name = "Salbutamol", Strength = "100mcg", Form = "Inhaler" },
                new Medication { Id = 8, Name = "Levothyroxine", Strength = "50mcg", Form = "Tablet" },
                new Medication { Id = 9, Name = "Losartan", Strength = "50mg", Form = "Tablet" },
                new Medication { Id = 10, Name = "Diclofenac", Strength = "50mg", Form = "Tablet" },
                new Medication { Id = 11, Name = "Cetirizine", Strength = "10mg", Form = "Tablet" },
                new Medication { Id = 12, Name = "Fluticason", Strength = "50mcg", Form = "Neusspray" }
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