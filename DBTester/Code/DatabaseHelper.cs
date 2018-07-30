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
using FrgxPublicApiSDK.Models;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace DBTester.Code
{
    public class DatabaseHelper
    {
        public static DataTable MakeUPCTable()
        {
            DataTable upcTable = new DataTable("UPC");

            ColumnMaker(upcTable, "Item", "System.Int32");
            ColumnMaker(upcTable, "Upc", "System.Int64");
            //DataColumn item = new DataColumn();
            //item.DataType = System.Type.GetType("System.Int32");
            //item.ColumnName = "Item";
            //upcTable.Columns.Add(item);

            //DataColumn upc = new DataColumn();
            //upc.DataType = System.Type.GetType("System.Int64");
            //upc.ColumnName = "Upc";
            //upcTable.Columns.Add(upc);

            return upcTable;
        }

        public static DataTable MakeFragrancexTable()
        {
            DataTable fragrancexTable = new DataTable("Fragrancex");

            ColumnMaker(fragrancexTable, "ItemID", "System.Int32");
            ColumnMaker(fragrancexTable, "BrandName", "System.String");
            ColumnMaker(fragrancexTable, "Description", "System.String");
            ColumnMaker(fragrancexTable, "Gender", "System.String");
            ColumnMaker(fragrancexTable, "Instock", "System.Boolean");
            ColumnMaker(fragrancexTable, "LargeImageUrl", "System.String");
            ColumnMaker(fragrancexTable, "MetricSize", "System.String");
            ColumnMaker(fragrancexTable, "ParentCode", "System.String");
            ColumnMaker(fragrancexTable, "ProductName", "System.String");
            ColumnMaker(fragrancexTable, "RetailPriceUSD", "System.Int32");
            ColumnMaker(fragrancexTable, "Size", "System.String");
            ColumnMaker(fragrancexTable, "SmallImageURL", "System.String");
            ColumnMaker(fragrancexTable, "Type", "System.String");
            ColumnMaker(fragrancexTable, "Upc", "System.Int64");
            ColumnMaker(fragrancexTable, "WholePriceAUD", "System.Double");
            ColumnMaker(fragrancexTable, "WholePriceCAD", "System.Double");
            ColumnMaker(fragrancexTable, "WholePriceEUR", "System.Double");
            ColumnMaker(fragrancexTable, "WholePriceGBP", "System.Double");
            ColumnMaker(fragrancexTable, "WholePriceUSD", "System.Double");
            ColumnMaker(fragrancexTable, "UpcItemID", "System.Double");

            return fragrancexTable;
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

            IConfiguration Configuration;
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

        public static void dbPreparer(DataTable uploadFragrancex, Dictionary<int, long?> upc, ref int bulkSize, List<Product> allProducts)
        {
            long? value = 0;
            
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

        public static void FragrancexLoadDic(string path, Dictionary<int, long?> upc)
        {
            List<UPC> list = new List<UPC>();

            FileInfo file = new FileInfo(path);

            DataTable uploadFragrancex = DatabaseHelper.MakeFragrancexTable();

            int bulkSize = 0;

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    long? value = 0;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (row != 1)
                        {
                            //upc.ItemID = Convert.ToInt32(worksheet.Cells[row, 1].Value?.ToString());
                            //upc.Upc = Convert.ToInt64(worksheet.Cells[row, 2].Value?.ToString());

                            DataRow insideRow = uploadFragrancex.NewRow();

                            insideRow["ItemID"] = Convert.ToInt32(worksheet.Cells[row, 1].Value?.ToString());
                            insideRow["BrandName"] = null;
                            insideRow["Description"] = null;
                            insideRow["Gender"] = null;
                            insideRow["Instock"] = true;
                            insideRow["LargeImageUrl"] = null;
                            insideRow["MetricSize"] = null;
                            insideRow["ParentCode"] = null;
                            insideRow["ProductName"] = null;
                            insideRow["RetailPriceUSD"] = 0.0;
                            insideRow["Size"] = null;
                            insideRow["SmallImageURL"] = null;
                            insideRow["Type"] = null;
                            insideRow["WholePriceAUD"] = 0.0;
                            insideRow["WholePriceCAD"] = 0.0;
                            insideRow["WholePriceEUR"] = 0.0;
                            insideRow["WholePriceGBP"] = 0.0;
                            insideRow["WholePriceUSD"] = Convert.ToDouble(worksheet.Cells[row, 6].Value?.ToString());

                            if (upc.TryGetValue(Convert.ToInt32(Convert.ToInt32(worksheet.Cells[row, 1].Value?.ToString())), out value))
                            {
                                insideRow["Upc"] = value;
                            }

                            //insideRow["UpcItemID"] = Convert.ToInt32(product.ItemId);

                            uploadFragrancex.Rows.Add(insideRow);
                            uploadFragrancex.AcceptChanges();

                            bulkSize++;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            DatabaseHelper.upload(uploadFragrancex, bulkSize, "dbo.Fragrancex");
        }
    }
}
