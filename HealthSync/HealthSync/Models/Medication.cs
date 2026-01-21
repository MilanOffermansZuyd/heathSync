using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthSync.Models
{
    public class Medication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Strength { get; set; }
        public string Form { get; set; }
    }
}
