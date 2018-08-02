using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class AzImporter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemID { get; set; }

        public string Sku { get; set; }

        public string Category { get; set; }

        public string ItemName { get; set; }

        public string Image1 { get; set; }

        public string Image2 { get; set; }

        public string Image3 { get; set; }

        public string Image4 { get; set; }

        public string Image5 { get; set; }

        public string Image6 { get; set; }

        public string Image7 { get; set; }

        public string Image8 { get; set; }

        public string MainImage { get; set; }

        public double WholeSale { get; set; }

        public int Quantity { get; set; }

        public int Weight { get; set; }

        public string HTMLDescription { get; set; }

        public string ShortDescription { get; set; }

    }
}
