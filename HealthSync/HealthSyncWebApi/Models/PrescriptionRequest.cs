using HealthSyncWebApi.Models.Enums;

namespace HealthSyncWebApi.Models
{
    public class PrescriptionRequest
    {
        public int Id { get; set; }

        public Guid ClientRequestId { get; set; }

        public int UserId { get; set; }
        public int MedicationId { get; set; }
        public int DoctorId { get; set; }
        public int PharmacyId { get; set; }

        public string? Note { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime DateOfRequest { get; set; } = DateTime.Now;
        public DateTime? DateOfResponse { get; set; }
        public int? ApprovedPrescriptionId { get; set; }
        public Prescription? ApprovedPrescription { get; set; }
    }
}
