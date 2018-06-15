using DBTester.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTester.Code
{
    public class UpcHelper
    {
        public static void UPCLoadDic(string path)
        {
            UPC upc = new UPC();

            List<UPC> list = new List<UPC>();

            StringBuilder sb = new StringBuilder();

            FileInfo file = new FileInfo(path);

            DataTable uploadUpc = DatabaseHelper.MakeUPCTable();

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

            DatabaseHelper.upload(uploadUpc, bulkSize, "dbo.UPC");
        }
    }
}
