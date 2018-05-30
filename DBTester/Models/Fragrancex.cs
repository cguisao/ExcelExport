using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class Fragrancex
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemID { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public string Gender { get; set; }
        public bool Instock { get; set; }
        public string LargeImageUrl { get; set; }
        public string MetricSize { get; set; }
        public string ParentCode { get; set; }
        public string ProductName { get; set; }
        public double RetailPriceUSD { get; set; }
        public string Size { get; set; }
        public string SmallImageURL { get; set; }
        public string Type { get; set; }
        public double WholePriceAUD { get; set; }
        public double WholePriceCAD { get; set; }
        public double WholePriceEUR { get; set; }
        public double WholePriceGBP { get; set; }
        public double WholePriceUSD { get; set; }

        public long? Upc { get; set; }
        public UPC upc { get; set; }
        
    }
}