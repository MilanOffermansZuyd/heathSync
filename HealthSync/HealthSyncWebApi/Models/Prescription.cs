namespace HealthSyncWebApi.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int MedicationId { get; set; }
        public int DoctorId { get; set; }
        public int PharmacyId { get; set; }

        public DateTime DateOfPrescription { get; set; } = DateTime.Now;

        public int Amount { get; set; }
        public string Unit { get; set; } = "stuks";

        public int RefillsRemaining { get; set; }

        public int TakeCount { get; set; }
        public int TimeBetweenMinutes { get; set; }

        public string? Instruction { get; set; }
    }
}
