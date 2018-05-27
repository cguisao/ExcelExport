using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class UPC
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UpcID { get; set; }
        public long Upc { get; set; }
    }
}
