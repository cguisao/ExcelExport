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
        public DBModifierAzImporterExcel(string path)
        {
            this.path = path;
        }

        private string path { set; get; }

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

        public void TableExecutor()
        {
            FileInfo file = new FileInfo(path);

            DataTable uploadAzImporter = CreateTable();

            int bulkSize = 0;

            int exception = 0;

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (row != 1)
                        {
                            exception++;

                            DataRow insideRow = uploadAzImporter.NewRow();

                            insideRow["ItemID"] = row;
                            insideRow["Sku"] = worksheet.Cells[row, 1].Value?.ToString().ToUpper();
                            insideRow["Category"] = worksheet.Cells[row, 2].Value?.ToString();
                            insideRow["ItemName"] = worksheet.Cells[row, 3].Value?.ToString();
                            insideRow["Image1"] = worksheet.Cells[row, 4].Value?.ToString();
                            insideRow["Image2"] = worksheet.Cells[row, 5].Value?.ToString();
                            insideRow["Image3"] = worksheet.Cells[row, 6].Value?.ToString();
                            insideRow["Image4"] = worksheet.Cells[row, 7].Value?.ToString();
                            insideRow["Image5"] = worksheet.Cells[row, 8].Value?.ToString();
                            insideRow["Image6"] = worksheet.Cells[row, 9].Value?.ToString();
                            insideRow["Image7"] = worksheet.Cells[row, 10].Value?.ToString();
                            insideRow["Image8"] = worksheet.Cells[row, 11].Value?.ToString();
                            insideRow["MainImage"] = worksheet.Cells[row, 12].Value?.ToString();
                            insideRow["WholeSale"] = Convert.ToDouble(worksheet.Cells[row, 13].Value);
                            insideRow["Quantity"] = Convert.ToInt32(worksheet.Cells[row, 14].Value?.ToString());
                            insideRow["ShortDescription"] = worksheet.Cells[row, 25].Value?.ToString();
                            insideRow["Weight"] = Convert.ToDouble(worksheet.Cells[row, 15].Value?.ToString());
                            insideRow["HTMLDescription"] = worksheet.Cells[row, 24].Value?.ToString();
                             
                            uploadAzImporter.Rows.Add(insideRow);
                            uploadAzImporter.AcceptChanges();

                            bulkSize++;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            upload(uploadAzImporter, bulkSize, "dbo.AzImporter");
        }
    }
}
