using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class PerfumeWorldWide
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemID { get; set; }
        public string sku { get; set; }
        public string Brand { get; set; }
        public string Designer { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Gender { get; set; }
        public string Set { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double Cost { get; set; }
        public double? Weight { get; set; }
        public double? MSRP { get; set; }
        public long? upc { get; set; }
    }
}
