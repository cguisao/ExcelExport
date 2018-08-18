﻿// <auto-generated />
using DBTester.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace GTISolutions.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DBTester.Models.AzImporter", b =>
                {
                    b.Property<int>("ItemID");

                    b.Property<string>("Category");

                    b.Property<string>("HTMLDescription");

                    b.Property<string>("Image1");

                    b.Property<string>("Image2");

                    b.Property<string>("Image3");

                    b.Property<string>("Image4");

                    b.Property<string>("Image5");

                    b.Property<string>("Image6");

                    b.Property<string>("Image7");

                    b.Property<string>("Image8");

                    b.Property<string>("ItemName");

                    b.Property<string>("MainImage");

                    b.Property<int>("Quantity");

                    b.Property<string>("ShortDescription");

                    b.Property<string>("Sku");

                    b.Property<int>("Weight");

                    b.Property<double>("WholeSale");

                    b.HasKey("ItemID");

                    b.ToTable("AzImporter");
                });

            modelBuilder.Entity("DBTester.Models.ErrorViewModel", b =>
                {
                    b.Property<string>("RequestId")
                        .ValueGeneratedOnAdd();

                    b.HasKey("RequestId");

                    b.ToTable("ErrorViewModel");
                });

            modelBuilder.Entity("DBTester.Models.Fragrancex", b =>
                {
                    b.Property<int>("ItemID");

                    b.Property<string>("BrandName");

                    b.Property<string>("Description");

                    b.Property<string>("Gender");

                    b.Property<bool>("Instock");

                    b.Property<string>("LargeImageUrl");

                    b.Property<string>("MetricSize");

                    b.Property<string>("ParentCode");

                    b.Property<string>("ProductName");

                    b.Property<double>("RetailPriceUSD");

                    b.Property<string>("Size");

                    b.Property<string>("SmallImageURL");

                    b.Property<string>("Type");

                    b.Property<long?>("Upc");

                    b.Property<double>("WholePriceAUD");

                    b.Property<double>("WholePriceCAD");

                    b.Property<double>("WholePriceEUR");

                    b.Property<double>("WholePriceGBP");

                    b.Property<double>("WholePriceUSD");

                    b.Property<int?>("upcItemID");

                    b.HasKey("ItemID");

                    b.HasIndex("upcItemID");

                    b.ToTable("Fragrancex");
                });

            modelBuilder.Entity("DBTester.Models.PerfumeWorldWide", b =>
                {
                    b.Property<string>("sku");

                    b.Property<string>("Brand");

                    b.Property<double>("Cost");

                    b.Property<string>("Description");

                    b.Property<string>("Designer");

                    b.Property<string>("Gender");

                    b.Property<string>("Image");

                    b.Property<double>("MSRP");

                    b.Property<string>("Set");

                    b.Property<string>("Size");

                    b.Property<string>("Type");

                    b.Property<double>("Weight");

                    b.Property<long?>("upc");

                    b.HasKey("sku");

                    b.ToTable("PerfumeWorldWide");
                });

            modelBuilder.Entity("DBTester.Models.Profile", b =>
                {
                    b.Property<string>("ProfileUser");

                    b.Property<string>("LongstartTitle");

                    b.Property<string>("MidtartTitle");

                    b.Property<string>("ShortstartTitle");

                    b.Property<string>("endTtile");

                    b.Property<double>("fee");

                    b.Property<byte[]>("formFile");

                    b.Property<string>("html");

                    b.Property<int>("items");

                    b.Property<double>("markdown");

                    b.Property<int>("max");

                    b.Property<int>("min");

                    b.Property<double>("profit");

                    b.Property<double>("promoting");

                    b.Property<double>("shipping");

                    b.Property<string>("sizeDivider");

                    b.HasKey("ProfileUser");

                    b.ToTable("Profile");
                });

            modelBuilder.Entity("DBTester.Models.ServiceTimeStamp", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("TimeStamp");

                    b.Property<string>("Wholesalers");

                    b.Property<string>("type");

                    b.HasKey("id");

                    b.ToTable("ServiceTimeStamp");
                });

            modelBuilder.Entity("DBTester.Models.UPC", b =>
                {
                    b.Property<int>("ItemID");

                    b.Property<long?>("Upc");

                    b.HasKey("ItemID");

                    b.ToTable("UPC");
                });

            modelBuilder.Entity("DBTester.Models.Fragrancex", b =>
                {
                    b.HasOne("DBTester.Models.UPC", "upc")
                        .WithMany("fragrancex")
                        .HasForeignKey("upcItemID");
                });
#pragma warning restore 612, 618
        }
    }
}
