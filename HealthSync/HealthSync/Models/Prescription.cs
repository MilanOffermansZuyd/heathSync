using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthSync.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public Medication Medication { get; set; }
        public Pharmacy Pharmacy { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime DateOfPrescription { get; set; }
        public int Amount { get; set; }
        public string Unit { get; set; }
        public int RefillsRemaining { get; set; }
        public int TakeCount { get; set; }
        public int TimeBetweenMinutes { get; set; }
        public string Instruction { get; set; }
    }
}
