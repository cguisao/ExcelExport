using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }

        public string ProfileUser { get; set; }

        public string html { get; set; }

        public int items { get; set; }
        
        public int profit { get; set; }

        public int markdown { get; set; }

        public int min { get; set; }

        public int max { get; set; }

        public byte[] formFile  { get; set; }
    }
}
