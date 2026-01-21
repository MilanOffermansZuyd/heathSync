using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthSync.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Place { get; set; }
        public string Country { get; set; }
    }
}
