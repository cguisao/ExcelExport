using DBTester.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    public class DBModifierFragrancexExcel : Database, IDatabaseModifier
    {
        public DBModifierFragrancexExcel(string path, Dictionary<int, long?> upc)
        {
            this.path = path;
            this.upc = upc;
        }

        private string path { get; set; }

        private Dictionary<int, long?> upc { get; set; }

        public DataTable CreateTable()
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

        public virtual void TableExecutor()
        {

            {
                List<UPC> list = new List<UPC>();

                FileInfo file = new FileInfo(path);

                DataTable uploadFragrancex = CreateTable();

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

                upload(uploadFragrancex, bulkSize, "dbo.Fragrancex");
            }
        }
    }
}
