using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthSync.Models
{
    public class Pharmacy
    {
        public int Id { get; set; }
        public string PharmacyName { get; set; }
        public int EmployeeId { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
    }
}
