using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthSync.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
        public string Message { get; set; }
        public bool IsSent { get; set; }
    }
}
