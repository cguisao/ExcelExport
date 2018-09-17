using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    public class DBModifierPerfumeWorldWideExcel : Database, IDatabaseModifier
    {
        public DBModifierPerfumeWorldWideExcel(string path)
        {
            this.path = path;
        }

        private string path { set; get; }

        public DataTable CreateTable()
        {
            DataTable PerfumeWorldWideTable = new DataTable("PerfumeWorldWide");

            ColumnMaker(PerfumeWorldWideTable, "ItemID", "System.Int32");
            ColumnMaker(PerfumeWorldWideTable, "Brand", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Cost", "System.Double");
            ColumnMaker(PerfumeWorldWideTable, "Description", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Designer", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Gender", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Image", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "MSRP", "System.Double");
            ColumnMaker(PerfumeWorldWideTable, "Set", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Size", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Type", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Weight", "System.Double");
            ColumnMaker(PerfumeWorldWideTable, "sku", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "upc", "System.Int64");
            return PerfumeWorldWideTable;
        }

        public void TableExecutor()
        {
            FileInfo file = new FileInfo(path);

            DataTable uploadPerfumeWorldWide = CreateTable();

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

                            DataRow insideRow = uploadPerfumeWorldWide.NewRow();

                            insideRow["ItemID"] = bulkSize + 1;
                            insideRow["Brand"] = worksheet.Cells[row, 2].Value?.ToString();
                            insideRow["Cost"] = Convert.ToDouble(worksheet.Cells[row, 10].Value?.ToString());
                            insideRow["Description"] = worksheet.Cells[row, 8].Value?.ToString();
                            insideRow["Designer"] = worksheet.Cells[row, 3].Value?.ToString();
                            insideRow["Gender"] = worksheet.Cells[row, 6].Value?.ToString();
                            insideRow["Image"] = worksheet.Cells[row, 9].Value?.ToString();
                            insideRow["MSRP"] = Convert.ToDouble(worksheet.Cells[row, 12].Value?.ToString());
                            insideRow["Size"] = worksheet.Cells[row, 4].Value?.ToString();
                            insideRow["Set"] = worksheet.Cells[row, 7].Value?.ToString();
                            insideRow["Type"] = worksheet.Cells[row, 5].Value?.ToString();
                            insideRow["Weight"] = Convert.ToDouble(worksheet.Cells[row, 11].Value?.ToString());
                            insideRow["sku"] = worksheet.Cells[row, 1].Value?.ToString();
                            insideRow["upc"] = Convert.ToInt64(worksheet.Cells[row, 13].Value?.ToString());

                            uploadPerfumeWorldWide.Rows.Add(insideRow);
                            uploadPerfumeWorldWide.AcceptChanges();

                            bulkSize++;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            upload(uploadPerfumeWorldWide, bulkSize, "dbo.PerfumeWorldWide");
        }
    }
}
