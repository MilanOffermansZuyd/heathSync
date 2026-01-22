using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthSync.Models.Enums;

namespace HealthSync.Models
{
    public class PrescriptionRequest
    {
        public int Id { get; set; }

        // Unieke id vanuit de app (handig voor sync / dubbele requests voorkomen)
        public Guid ClientRequestId { get; set; } = Guid.NewGuid();
        // ID die je terugkrijgt van de API zodra het request daar is opgeslagen
        public int? RemoteId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
        public int MedicationId { get; set; }
        public Medication? Medication { get; set; }
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        public int PharmacyId { get; set; }
        public Pharmacy? Pharmacy { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime DateOfRequest { get; set; } = DateTime.UtcNow;
        public DateTime? DateOfResponse { get; set; }
        public int? ApprovedPrescriptionId { get; set; }
        public Prescription? ApprovedPrescription { get; set; }
    }
}
