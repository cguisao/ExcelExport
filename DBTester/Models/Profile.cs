using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class Profile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ProfileUser { get; set; }
        
        public int items { get; set; }

        public string html { get; set; }

        public int profit { get; set; }

        public int markdown { get; set; }

        public int shipping { get; set; }

        public int fee { get; set; }

        public int promoting { get; set; }

        public int min { get; set; }

        public int max { get; set; }

        public byte[] formFile  { get; set; }
    }
}
