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
    public class DBModifierAzImporterExcel : Database, IDatabaseModifier
    {
        public DBModifierAzImporterExcel(string _path, Dictionary<string, AzImporter> _azImportItems)
        {
            path = _path;
            azImportItems = _azImportItems;
        }

        private string path { set; get; }

        private Dictionary<string, AzImporter> azImportItems { get; set; }
        
        public DataTable CreateTable()
        {
            DataTable azImporterTable = new DataTable("AzImporter");

            ColumnMaker(azImporterTable, "ItemID", "System.Int32");
            ColumnMaker(azImporterTable, "Category", "System.String");
            ColumnMaker(azImporterTable, "HTMLDescription", "System.String");
            ColumnMaker(azImporterTable, "Image1", "System.String");
            ColumnMaker(azImporterTable, "Image2", "System.String");
            ColumnMaker(azImporterTable, "Image3", "System.String");
            ColumnMaker(azImporterTable, "Image4", "System.String");
            ColumnMaker(azImporterTable, "Image5", "System.String");
            ColumnMaker(azImporterTable, "Image6", "System.String");
            ColumnMaker(azImporterTable, "Image7", "System.String");
            ColumnMaker(azImporterTable, "Image8", "System.String");
            ColumnMaker(azImporterTable, "itemName", "System.String");
            ColumnMaker(azImporterTable, "MainImage", "System.String");
            ColumnMaker(azImporterTable, "Quantity", "System.Int32");
            ColumnMaker(azImporterTable, "ShortDescription", "System.String");
            ColumnMaker(azImporterTable, "Sku", "System.String");
            ColumnMaker(azImporterTable, "Weight", "System.Int32");
            ColumnMaker(azImporterTable, "WholeSale", "System.Double");

            return azImporterTable;
        }

        private void TableCreator()
        {
            FileInfo file = new FileInfo(path);
            
            int exception = 0;

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        AzImporter az = new AzImporter();
                        exception++;
                        if(exception == 10)
                        {

                        }
                        if (row != 1)
                        {
                            az.ItemID = row - 1;
                            az.Sku = worksheet.Cells[row, 1].Value?.ToString().ToUpper();
                            az.Category = worksheet.Cells[row, 2].Value?.ToString();
                            az.ItemName = worksheet.Cells[row, 3].Value?.ToString();
                            az.Image1 = worksheet.Cells[row, 4].Value?.ToString();
                            az.Image2 = worksheet.Cells[row, 5].Value?.ToString();
                            az.Image3 = worksheet.Cells[row, 6].Value?.ToString();
                            az.Image4 = worksheet.Cells[row, 7].Value?.ToString();
                            az.Image5 = worksheet.Cells[row, 8].Value?.ToString();
                            az.Image6 = worksheet.Cells[row, 9].Value?.ToString();
                            az.Image7 = worksheet.Cells[row, 10].Value?.ToString();
                            az.Image8 = worksheet.Cells[row, 11].Value?.ToString();
                            az.MainImage = worksheet.Cells[row, 12].Value?.ToString();
                            az.WholeSale = Convert.ToDouble(worksheet.Cells[row, 13].Value?.ToString());
                            az.Quantity = Convert.ToInt32(worksheet.Cells[row, 14]?.Value);
                            az.ShortDescription = worksheet.Cells[row, 25].Value?.ToString();
                            az.Weight = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(worksheet.Cells[row, 15]?.Value)));
                            az.HTMLDescription = worksheet.Cells[row, 24].Value?.ToString();

                            if(!azImportItems.TryAdd(az.Sku, az))
                            {
                                azImportItems[az.Sku] = az;
                            }
                            else
                            {
                                azImportItems.TryAdd(az.Sku, az);
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public void TableExecutor()
        {
            TableCreator();

            DataTable uploadAzImporter = CreateTable();

            int bulkSize = 0;

            int exception = 0;

            foreach(var az in azImportItems)
            {
                exception++;
                DataRow insideRow = uploadAzImporter.NewRow();

                insideRow["ItemID"] = bulkSize;
                insideRow["Sku"] = az.Value.Sku;
                insideRow["Category"] = az.Value.Category;
                insideRow["ItemName"] = az.Value.ItemName;
                insideRow["Image1"] = az.Value.Image1;
                insideRow["Image2"] = az.Value.Image2;
                insideRow["Image3"] = az.Value.Image3;
                insideRow["Image4"] = az.Value.Image4;
                insideRow["Image5"] = az.Value.Image5;
                insideRow["Image6"] = az.Value.Image6;
                insideRow["Image7"] = az.Value.Image7;
                insideRow["Image8"] = az.Value.Image8;
                insideRow["MainImage"] = az.Value.MainImage;
                insideRow["WholeSale"] = Convert.ToDouble(az.Value.WholeSale);
                insideRow["Quantity"] = Convert.ToInt32(az.Value.Quantity);
                insideRow["ShortDescription"] = az.Value.ShortDescription;
                insideRow["Weight"] = Convert.ToDouble(az.Value.Weight);
                insideRow["HTMLDescription"] = az.Value.HTMLDescription;

                uploadAzImporter.Rows.Add(insideRow);
                uploadAzImporter.AcceptChanges();

                bulkSize++;
            }

            upload(uploadAzImporter, bulkSize, "dbo.AzImporter");
        }
    }
}
