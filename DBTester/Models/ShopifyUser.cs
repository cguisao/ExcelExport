using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class ShopifyUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemID { get; set; }
        public string sku { get; set; }
        public string handle { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string vendor { get; set; }
        public string type { get; set; }
        public string option1Name { get; set; }
        public string option1Value { get; set; }
        public double price { get; set; }
        public double comparePrice { get; set; }
        public string image { get; set; }
        public string tags { get; set; }
        public string collection { get; set; }
        public long? upc { get; set; }
        public string userID { get; set; }
    }
}
