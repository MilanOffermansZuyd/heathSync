using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthSync.Models
{
    public class HealthData
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public double HeartRateInRest { get; set; }
        public DateTime StartSleep { get; set; }
        public DateTime EndSleep { get; set; }
        public double MinutesAwake { get; set; }
        public int StepCount { get; set; }
    }
}
