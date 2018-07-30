using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBTester.Models;
using OfficeOpenXml;

namespace ExcelModifier
{
    public class ShopifyExcelUpdator : IExcelExtension
    {
        public string sWebRootFolder { get; set; }
        public Dictionary<int, double> prices { get; set; }
        public Profile profile { get; set; }

        public void ExcelGenerator()

        {
            FileInfo file = new FileInfo(sWebRootFolder);
            Dictionary<string, long> dicSKU = new Dictionary<string, long>();
            Dictionary<string, string> dicTitle = new Dictionary<string, string>();

            int execption = 0;
            int itemsCol = 0;
            int itemQty = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    int rowCount = worksheet.Dimension.Rows;

                    int ColCount = worksheet.Dimension.Columns;

                    long? itemID;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        execption++;
                        if (row == 1)
                        {
                            for (int col = 1; col <= ColCount; col++)
                            {
                                if (worksheet.Cells[row, col].Value.ToString() == "Variant SKU")
                                {
                                    itemsCol = col;
                                    continue;
                                }
                                else if (worksheet.Cells[row, col].Value.ToString() == "Variant Inventory Qty")
                                {
                                    itemQty = col;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            double value;

                            itemID = Convert.ToInt64(worksheet.Cells[row, itemsCol].Value.ToString().Replace("'", ""));

                            worksheet.Cells[row, itemsCol].Value = itemID;

                            if (prices.TryGetValue(Convert.ToInt32(itemID), out value))
                            {
                                worksheet.Cells[row, itemQty].Value = profile.items;
                            }
                            else
                            {
                                worksheet.Cells[row, itemQty].Value = 0;
                            }
                        }
                    }
                    package.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred while importing." + ex.Message);
            }
        }
    }
}
