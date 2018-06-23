using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        { }

        public DbSet<Profile> Profile { set; get; }
        public DbSet<ServiceTimeStamp> ServiceTimeStamp { get; set; }
        public DbSet<Fragrancex> Fragrancex { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<UPC> UPC { get; set; }
    }
    
}
