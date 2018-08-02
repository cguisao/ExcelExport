using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class ServiceTimeStamp
    {
        [Key]
        public int id { get; set; }

        public DateTime TimeStamp { get; set; }

        public string type { get; set; }

        public string Wholesalers { get; set; }
    }
}
