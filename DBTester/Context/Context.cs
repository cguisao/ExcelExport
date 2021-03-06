﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Profile> Profile { set; get; }
        public DbSet<ServiceTimeStamp> ServiceTimeStamp { get; set; }
        public DbSet<Fragrancex> Fragrancex { get; set; }
        public DbSet<UPC> UPC { get; set; }
        public DbSet<AzImporter> AzImporter { get; set; }
        public DbSet<PerfumeWorldWide> PerfumeWorldWide { get; set; }
        public DbSet<Shipping> Shipping { get; set; }
        public DbSet<Amazon> Amazon { get; set; }
        public DbSet<FragrancexTitle> FragrancexTitle { get; set; }
        public DbSet<ErrorViewModel> ErrorViewModel { get; set; }
        public DbSet<ShopifyUser> ShopifyUser { get; set; }
        public DbSet<UsersList> UsersList { get; set; }
        public DbSet<UsersListTemp> UsersListTemp { get; set; }
    }
}
