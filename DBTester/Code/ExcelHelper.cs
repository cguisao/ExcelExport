using DBTester.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTester.Code
{
    public class ExcelHelper
    {
        public static void ExcelGenerator(string sWebRootFolder, Dictionary<int, double> prices,
            Dictionary<int, long?> upcs, Profile profile, int items
            , int min, int max)
        {
            FileInfo file = new FileInfo(sWebRootFolder);
            Dictionary<string, long> dicSKU = new Dictionary<string, long>();
            Dictionary<string, string> dicTitle = new Dictionary<string, string>();
            int count = 1;
            int execption = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    worksheet.DeleteRow(1);

                    worksheet.Cells["A:AA"].Sort();

                    worksheet.InsertRow(1, 1);

                    // Prepare the excel and remove whatever it needs to be removed.

                    Helper.PrepareExcel(worksheet, min, max);

                    dicTitle = Helper.titleDic(worksheet, profile);

                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;

                    long? itemID;

                    string title = "";

                    for (int row = 1; row <= rowCount + 1; row++)
                    {
                        execption++;

                        if (row == 1)
                        {
                            worksheet.Cells[row, 1].Value = "Handle";
                            worksheet.Cells[row, 2].Value = "Title";
                            worksheet.Cells[row, 3].Value = "Body (HTML)";
                            worksheet.Cells[row, 4].Value = "Vendor";
                            worksheet.Cells[row, 5].Value = "Type";
                            worksheet.Cells[row, 6].Value = "Published";
                            worksheet.Cells[row, 7].Value = "Option1 Name";
                            worksheet.Cells[row, 8].Value = "Option1 Value";
                            worksheet.Cells[row, 9].Value = "OPtion2 Name";
                            worksheet.Cells[row, 10].Value = "Option2 Value";
                            worksheet.Cells[row, 11].Value = "Option3 Name";
                            worksheet.Cells[row, 12].Value = "Option3 Value";
                            worksheet.Cells[row, 13].Value = "Variant SKU";
                            worksheet.Cells[row, 14].Value = "Variant Grams";
                            worksheet.Cells[row, 15].Value = "Variant Inventory Tracker";
                            worksheet.Cells[row, 16].Value = "Variant Inventory Qty";
                            worksheet.Cells[row, 17].Value = "Variant Inventory Policy";
                            worksheet.Cells[row, 18].Value = "Variant Fulfillment Service";
                            worksheet.Cells[row, 19].Value = "Variant Price";
                            worksheet.Cells[row, 20].Value = "Variant Compare At Price";
                            worksheet.Cells[row, 21].Value = "Variant Requires Shipping";
                            worksheet.Cells[row, 22].Value = "Variant Taxable";
                            worksheet.Cells[row, 23].Value = "Variant Barcode";
                            worksheet.Cells[row, 24].Value = "Image Src";
                            worksheet.Cells[row, 25].Value = "Image Alt Text";
                            worksheet.Cells[row, 26].Value = "Tags";
                            worksheet.Cells[row, 27].Value = "Collection";
                            worksheet.Cells[row, 28].Value = "Price from Database (DELETE THE COLUMN!)";
                        }
                        else
                        {
                            // Logic for the title
                            title = Helper.BuildTitle(dicTitle, worksheet.Cells[row, 2].Value.ToString()
                                + " " + worksheet.Cells[row, 27].Value.ToString(), worksheet.Cells[row, 27].Value.ToString(), profile);
                            worksheet.Cells[row, 2].Value = title;
                            if (title.Length > 80)
                                count++;

                            //Logic for the HTML Body
                            
                            worksheet.Cells[row, 3].Value = Helper.BuildHTML(worksheet, row, profile.html);
                            
                            // SKU creator

                            itemID = Convert.ToInt64(worksheet.Cells[row, 13].Value);

                            long? value;

                            if (upcs.TryGetValue(Convert.ToInt32(itemID), out value))
                            {
                                worksheet.Cells[row, 23].Value = value;
                            }

                            // prices

                            string price = Helper.PricePreparer(itemID, prices, profile);

                            if (double.Parse(price) != 0.0)
                            {
                                worksheet.Cells[row, 19].Value = double.Parse(price);
                                worksheet.Cells[row, 16].Value = items;
                            }
                            else
                            {
                                worksheet.Cells[row, 19].Value = 100.0;
                                worksheet.Cells[row, 16].Value = 0;
                            }

                            // This logic fixes the picture in some cases

                            worksheet.Cells[row, 24].Value =
                                      worksheet.Cells[row, 24].Value.ToString()
                                          .Replace("http://img.fragrancex.com/images/products/SKU/small/"
                                          , "http://img.fragrancex.com/images/products/SKU/large/")
                                          .Replace("http", "https");

                            double actualPrice = 0.0;
                            prices.TryGetValue(Convert.ToInt32(itemID), out actualPrice);
                            worksheet.Cells[row, 28].Value = actualPrice;
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

        public static void ExcelGenerator(string sWebRootFolder, Dictionary<int, double> prices, int items)
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
                        if(row == 1)
                        {
                            for(int col = 1; col <= ColCount; col++)
                            {
                                if(worksheet.Cells[row, col].Value.ToString() == "Variant SKU")
                                {
                                    itemsCol = col;
                                    continue;
                                }
                                else if(worksheet.Cells[row, col].Value.ToString() == "Variant Inventory Qty")
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
                                worksheet.Cells[row, itemQty].Value = items;
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
