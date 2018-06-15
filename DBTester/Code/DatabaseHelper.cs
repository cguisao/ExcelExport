using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBTester.Models;
using FrgxPublicApiSDK;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace DBTester.Code
{
    public class DatabaseHelper
    {
        public static IConfiguration Configuration;

        public static DataTable MakeUPCTable()
        {
            DataTable upcTable = new DataTable("UPC");

            DataColumn item = new DataColumn();
            item.DataType = System.Type.GetType("System.Int32");
            item.ColumnName = "Item";
            upcTable.Columns.Add(item);

            DataColumn upc = new DataColumn();
            upc.DataType = System.Type.GetType("System.Int64");
            upc.ColumnName = "Upc";
            upcTable.Columns.Add(upc);

            return upcTable;
        }

        public static DataTable MakeFragrancexTable()
        {
            DataTable upcTable = new DataTable("Fragrancex");

            ColumnMaker(upcTable, "ItemID", "System.Int32");
            ColumnMaker(upcTable, "BrandName", "System.String");
            ColumnMaker(upcTable, "Description", "System.String");
            ColumnMaker(upcTable, "Gender", "System.String");
            ColumnMaker(upcTable, "Instock", "System.Boolean");
            ColumnMaker(upcTable, "LargeImageUrl", "System.String");
            ColumnMaker(upcTable, "MetricSize", "System.String");
            ColumnMaker(upcTable, "ParentCode", "System.String");
            ColumnMaker(upcTable, "ProductName", "System.String");
            ColumnMaker(upcTable, "RetailPriceUSD", "System.Int32");
            ColumnMaker(upcTable, "Size", "System.String");
            ColumnMaker(upcTable, "SmallImageURL", "System.String");
            ColumnMaker(upcTable, "Type", "System.String");
            ColumnMaker(upcTable, "Upc", "System.Int64");
            ColumnMaker(upcTable, "WholePriceAUD", "System.Double");
            ColumnMaker(upcTable, "WholePriceCAD", "System.Double");
            ColumnMaker(upcTable, "WholePriceEUR", "System.Double");
            ColumnMaker(upcTable, "WholePriceGBP", "System.Double");
            ColumnMaker(upcTable, "WholePriceUSD", "System.Double");
            ColumnMaker(upcTable, "UpcItemID", "System.Double");

            return upcTable;
        }

        private static void ColumnMaker(DataTable upcTable, string columnName, string type)
        {
            DataColumn item = new DataColumn();
            item.DataType = System.Type.GetType(type);
            item.ColumnName = columnName;
            upcTable.Columns.Add(item);
        }

        public static void upload(DataTable dataTable, int bulkSize, string tableName)
        {
            var builder = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                         .AddEnvironmentVariables();

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            string connectionstring = Configuration.GetConnectionString("BloggingDatabase");



            using (SqlConnection sourceConnection =
                   new SqlConnection(connectionstring))
            {
                sourceConnection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionstring))
                {
                    bulkCopy.DestinationTableName = tableName;

                    // Set the BatchSize.
                    bulkCopy.BatchSize = bulkSize;

                    try
                    {
                        // Write from the source to the destination.
                        bulkCopy.WriteToServer(dataTable);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public static void dbPreparer(DataTable uploadFragrancex, Dictionary<int, long?> upc, ref int bulkSize)
        {
            long? value = 0;
            var listingApiClient = new FrgxListingApiClient("346c055aaefd", "a5574c546cbbc9c10509e3c277dd7c7039b24324");

            Fragrancex fragrancex = new Fragrancex();

            var allProducts = listingApiClient.GetAllProducts();


            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var product in allProducts)
            {
                try
                {
                    dic.Add(product.ItemId, product.ProductName);
                    if (product != null)
                    {
                        DataRow insideRow = uploadFragrancex.NewRow();

                        insideRow["ItemID"] = Convert.ToInt32(product.ItemId);
                        insideRow["BrandName"] = product.BrandName;
                        insideRow["Description"] = product.Description;
                        insideRow["Gender"] = product.Gender;
                        insideRow["Instock"] = product.Instock;
                        insideRow["LargeImageUrl"] = product.LargeImageUrl;
                        insideRow["MetricSize"] = product.MetricSize;
                        insideRow["ParentCode"] = product.ParentCode;
                        insideRow["ProductName"] = product.ProductName;
                        insideRow["RetailPriceUSD"] = product.RetailPriceUSD;
                        insideRow["Size"] = product.Size;
                        insideRow["SmallImageURL"] = product.SmallImageUrl;
                        insideRow["Type"] = product.Type;
                        insideRow["WholePriceAUD"] = product.WholesalePriceAUD;
                        insideRow["WholePriceCAD"] = product.WholesalePriceCAD;
                        insideRow["WholePriceEUR"] = product.WholesalePriceEUR;
                        insideRow["WholePriceGBP"] = product.WholesalePriceGBP;
                        insideRow["WholePriceUSD"] = product.WholesalePriceUSD;

                        if (upc.TryGetValue(Convert.ToInt32(product.ItemId), out value))
                        {
                            insideRow["Upc"] = value;
                        }

                        insideRow["UpcItemID"] = Convert.ToInt32(product.ItemId);

                        uploadFragrancex.Rows.Add(insideRow);
                        uploadFragrancex.AcceptChanges();
                        bulkSize++;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
