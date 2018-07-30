using DBTester.Code;
using DBTester.Models;
using FrgxPublicApiSDK.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    public class DBModifierUPC : Database, IDatabaseModifier
    {
        public DBModifierUPC(string path)
        {
            this.path = path;
        }

        private string path { get; set; }

        public void TableExecutor()

        {
            UPC upc = new UPC();

            FileInfo file = new FileInfo(path);

            DataTable uploadUpc = CreateTable();

            int bulkSize = 0;

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (row != 1)
                        {
                            upc.ItemID = Convert.ToInt32(worksheet.Cells[row, 1].Value?.ToString());
                            upc.Upc = Convert.ToInt64(worksheet.Cells[row, 2].Value?.ToString());
                            if (upc.Upc == 0)
                                upc.Upc = -1;

                            DataRow insideRow = uploadUpc.NewRow();

                            insideRow["Item"] = upc.ItemID;
                            insideRow["Upc"] = upc.Upc;
                            uploadUpc.Rows.Add(insideRow);
                            uploadUpc.AcceptChanges();
                            bulkSize++;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            upload(uploadUpc, bulkSize, "dbo.UPC");
        }

        public DataTable CreateTable()
        {
            DataTable upcTable = new DataTable("UPC");

            ColumnMaker(upcTable, "Item", "System.Int32");
            ColumnMaker(upcTable, "Upc", "System.Int64");

            return upcTable;
        }
    }
}
