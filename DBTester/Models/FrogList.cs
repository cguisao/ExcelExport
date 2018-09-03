using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class FrogList
    {
        public long? sku { get; set; }
        
        public string description { get; set; }

        public double price { get; set; }

        public string pictures { get; set; }

        public string size { get; set; }

        public string fragranceType { get; set; }

        public string brand { get; set; }
        
        public string collection { get; set; }
    }
}
