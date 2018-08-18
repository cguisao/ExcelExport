using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExcelModifier
{
    public class PerfumeWorldWideComparer : IExcelExtension
    {
        public string path { get; set; }
        public Dictionary<int, double> fragrancexPrices { get; set; }

        public Dictionary<int, long?> fragrancexUpc { get; set; }
        
        public void ExcelGenerator()
        {
            FileInfo file = new FileInfo(path);
            long? skuID;
            int execption = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    long? sku = 0;
                    int counter = 0;

                    Parallel.For(1, rowCount, row =>
                    {
                        if (row != 1)
                        {
                            sku = Convert.ToInt64(worksheet.Cells[row, 13].Value);
                            if (IsFragrancex(sku))
                            {
                                worksheet.Cells[row, 1, row, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[row, 1, row, 14].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                int progress = Interlocked.Increment(ref counter);
                            }
                        }
                    });
                    worksheet.Cells[rowCount + 1, 1].Value = "Matched SKU";
                    worksheet.Cells[rowCount + 1, 2].Value = counter;

                    package.Save();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private bool IsFragrancex(long? sku)
        {
            if(fragrancexUpc.ContainsValue(sku))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string getSellingPrice(long? itemID)
        {
            throw new NotImplementedException();
        }
    }
}
