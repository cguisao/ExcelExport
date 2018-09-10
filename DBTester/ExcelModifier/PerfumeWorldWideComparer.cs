using DBTester.Models;
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
        public PerfumeWorldWideComparer(Dictionary<int, string> _fragrancex)
        {
            fragrancex = _fragrancex;
            fragrancexTitles = new Dictionary<int, string>();
            createDictionary();
        }

        public string path { get; set; }

        public Dictionary<int, double> fragrancexPrices { get; set; }

        public Dictionary<int, string> fragrancex { get; set; }

        public Dictionary<int, long?> fragrancexUpc { get; set; }
        
        public Dictionary<int, string> fragrancexTitles { get; set; }

        public void ExcelGenerator()
        {
            FileInfo file = new FileInfo(path);
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    long? sku = 0;
                    int counterBySKU = 0;
                    int counterByTitle = 0;
                    int counterByBlackListed = 0;
                    int counterByPrice = 0;
                    string title = string.Empty;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        sku = Convert.ToInt64(worksheet.Cells[row, 13].Value);
                        string brand = worksheet.Cells[row, 2].Value.ToString();
                        string designer = worksheet.Cells[row, 3].Value.ToString();
                        string size = worksheet.Cells[row, 4].Value.ToString();
                        string type = worksheet.Cells[row, 5].Value.ToString();
                        double price = Convert.ToDouble(worksheet.Cells[row, 10].Value);

                        title = brand + " " + type + " " + "By " + designer + " " + size;

                        if (IsBlackListedTitle(title))
                        {
                            worksheet.Cells[row, 1, row, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, 1, row, 14].Style.Fill.BackgroundColor.SetColor(Color.Purple);
                            int progress = Interlocked.Increment(ref counterByBlackListed);
                        }
                        else if (IsFragrancexBySKU(sku))
                        {
                            worksheet.Cells[row, 1, row, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, 1, row, 14].Style.Fill.BackgroundColor.SetColor(Color.Red);
                            int progress = Interlocked.Increment(ref counterBySKU);
                        }
                        else if (IsFragranceByTitle(title))
                        {
                            worksheet.Cells[row, 1, row, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, 1, row, 14].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                            int progress = Interlocked.Increment(ref counterByTitle);
                        }
                        else if(IsPriceUnacceptable(price))
                        {
                            worksheet.Cells[row, 1, row, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, 1, row, 14].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                            int progress = Interlocked.Increment(ref counterByPrice);
                        }
                    }

                    //Parallel.For(2, rowCount, row =>
                    //{
                    //    sku = Convert.ToInt64(worksheet.Cells[row, 13].Value);
                    //    string brand = worksheet.Cells[row, 2].Value.ToString();
                    //    string designer = worksheet.Cells[row, 3].Value.ToString();
                    //    string size = worksheet.Cells[row, 4].Value.ToString();
                    //    string type = worksheet.Cells[row, 5].Value.ToString();
                    //    title = brand + " " + type + " " + "By " + designer + " " + size;
                    //    //worksheet.Cells[row, 1, row, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    //    if (IsBlackListedTitle(title))
                    //    {
                    //        worksheet.Cells[row, 1, row, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //        worksheet.Cells[row, 1, row, 14].Style.Fill.BackgroundColor.SetColor(Color.Purple);
                    //        int progress = Interlocked.Increment(ref counterByBlackListed);
                    //    }
                    //    else if (IsFragrancexBySKU(sku))
                    //    {
                    //        worksheet.Cells[row, 1, row, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //        worksheet.Cells[row, 1, row, 14].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    //        int progress = Interlocked.Increment(ref counterBySKU);
                    //    }
                    //    else if (IsFragranceByTitle(title))
                    //    {
                    //        worksheet.Cells[row, 1, row, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //        worksheet.Cells[row, 1, row, 14].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                    //        int progress = Interlocked.Increment(ref counterByTitle);
                    //    }
                    //});

                    //worksheet.Cells[rowCount + 1, 1, rowCount + 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //worksheet.Cells[rowCount + 1, 1, rowCount + 1, 2].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    worksheet.Cells[rowCount + 1, 1].Value = "Matched SKU";
                    worksheet.Cells[rowCount + 1, 2].Value = counterBySKU;

                    //worksheet.Cells[rowCount + 2, 1, rowCount + 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //worksheet.Cells[rowCount + 2, 1, rowCount + 1, 2].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                    worksheet.Cells[rowCount + 2, 1].Value = "Matched Title";
                    worksheet.Cells[rowCount + 2, 2].Value = counterByTitle;

                    //worksheet.Cells[rowCount + 3, 1, rowCount + 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //worksheet.Cells[rowCount + 3, 1, rowCount + 1, 2].Style.Fill.BackgroundColor.SetColor(Color.Purple);
                    worksheet.Cells[rowCount + 3, 1].Value = "Black Listed Titles";
                    worksheet.Cells[rowCount + 3, 2].Value = counterByBlackListed;

                    worksheet.Cells[rowCount + 4, 1].Value = "Unacceptable Price";
                    worksheet.Cells[rowCount + 4, 2].Value = counterByPrice;

                    package.Save();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsPriceUnacceptable(double price)
        {
            if(price >= 150 || price <= 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsBlackListedTitle(string title)
        {
            if(title.ToLower().Contains("sample") || title.ToLower().Contains("tester")
                || title.ToLower().Contains("unboxed") || title.ToLower().Contains("vial"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsFragranceByTitle(string title)
        {
            if(fragrancex.ContainsValue(title))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void createDictionary()
        {
            foreach(var item in fragrancex)
            {
                fragrancexTitles.Add(item.Key, item.Value);
            }
        }

        private bool IsFragrancexBySKU(long? sku)
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
