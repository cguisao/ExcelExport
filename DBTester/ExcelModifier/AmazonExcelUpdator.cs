using DatabaseModifier;
using DBTester.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExcelModifier
{
    public class AmazonExcelUpdator : WholesaleHelper, IExcelExtension
    {
        public string path { get; set; }

        public Dictionary<int, double> fragrancexPrices { get; set; }
        
        public void ExcelGenerator()
        {
            FileInfo file = new FileInfo(path);
            long? skuID;
            int execption = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    int sku = 0;
                    int price = 0;
                    int quantity = 0;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        execption++;

                        if (row == 1)
                        {
                            for(int column = 1; column <= ColCount; column++)
                            {
                                if(worksheet.Cells[row, column].Value.ToString().ToLower().Contains("sku"))
                                {
                                    sku = column;
                                }
                                else if(worksheet.Cells[row, column].Value.ToString().ToLower().Contains("price"))
                                {
                                    price = column;
                                }
                                else if (worksheet.Cells[row, column].Value.ToString().ToLower().Contains("quantity"))
                                {
                                    quantity = column;
                                }
                            }

                            worksheet.Cells[row, 1].Value = "sku";
                            worksheet.Cells[row, 2].Value = "price";
                            worksheet.Cells[row, 3].Value = "minimum-seller-allowed-price";
                            worksheet.Cells[row, 4].Value = "maximum-seller-allowed-price";
                            worksheet.Cells[row, 5].Value = "quantity";
                            worksheet.Cells[row, 6].Value = "handling-time";
                            worksheet.Cells[row, 7].Value = "fulfillment-channel";
                            worksheet.Cells[row, 8].Value = "Selling Price";
                            worksheet.Cells[row, 9].Value = "Weight Price";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 1].Value?.ToString()))
                            {
                                // if the first row is a perfume/Cologne
                                string rowSku = worksheet.Cells[row, 1].Value.ToString();
                                long? digitSku = DigitGetter(rowSku);
                                double rowPrice = Convert.ToDouble(worksheet.Cells[row, price].Value);

                                if (isFragrancex(digitSku))
                                {
                                    skuID = DigitGetter(rowSku);
                                    
                                    double sellingPrice = Convert.ToDouble(getSellingPrice(skuID));
                                    
                                    // Price lower
                                    if (isPriceLower(rowPrice, sellingPrice) && sellingPrice != 0)
                                    {
                                        worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                        worksheet.Cells[row, 5].Value = worksheet.Cells[row, quantity].Value;
                                        worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                        worksheet.Cells[row, 8].Value = sellingPrice;
                                    }
                                    // The price is too high 
                                    else if(isPriceTooHigh(rowPrice, sellingPrice) && sellingPrice != 0)
                                    {
                                        worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                        worksheet.Cells[row, 5].Value = 3;
                                        worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Blue);
                                        worksheet.Cells[row, 8].Value = sellingPrice;
                                    }
                                    // In-stock
                                    else if (sellingPrice != 0)
                                    {
                                        worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                        worksheet.Cells[row, 5].Value = 3;
                                        worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                        worksheet.Cells[row, 8].Value = sellingPrice;
                                    }
                                    // Out of stock
                                    else
                                    {
                                        worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                        worksheet.Cells[row, 5].Value = 0;
                                        worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                    }
                                }
                                else if(isAzImporter(rowSku))
                                {
                                    // Weight is not register
                                    if (!isWeightRegister())
                                    {
                                        worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                        worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                                        worksheet.Cells[row, 8].Value = "Weight Not Register";
                                    }
                                    else
                                    {
                                        double sellingPrice = getSellingPrice();
                                        // Price too low
                                        if (isPriceLower(rowPrice, sellingPrice) && sellingPrice != 0)
                                        {
                                            worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                            worksheet.Cells[row, 5].Value = worksheet.Cells[row, quantity].Value;
                                            worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                            worksheet.Cells[row, 8].Value = sellingPrice;
                                            worksheet.Cells[row, 9].Value = AzImporterPriceWeight;
                                        }

                                        // Price is too high 
                                        else if (isPriceTooHigh(rowPrice, sellingPrice) && sellingPrice != 0)
                                        {
                                            worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                            worksheet.Cells[row, 5].Value = 3;
                                            worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Blue);
                                            worksheet.Cells[row, 9].Value = AzImporterPriceWeight;
                                        }
                                        // In-stock
                                        else if (sellingPrice != 0)
                                        {
                                            worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                            worksheet.Cells[row, 5].Value = 3;
                                            worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                            worksheet.Cells[row, 8].Value = sellingPrice;
                                            worksheet.Cells[row, 9].Value = AzImporterPriceWeight;
                                        }
                                        // Out of stock
                                        else
                                        {
                                            worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                            worksheet.Cells[row, 5].Value = 0;
                                            worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                            worksheet.Cells[row, 9].Value = AzImporterPriceWeight;
                                        }
                                    }
                                    azImporterSku = "";
                                    AzImporterPriceWeight = 0.0;
                                }
                                else
                                {
                                    worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                    worksheet.Cells[row, 5].Value = worksheet.Cells[row, quantity].Value;
                                }

                                worksheet.Cells[row, 3].Value = "";
                                worksheet.Cells[row, 4].Value = "";
                            }
                        }
                    }
                    
                    package.Save();
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        private bool isPriceTooHigh(double rowPrice, double sellingPrice)
        {
            if (sellingPrice == 0)
                return false;

            // 90% more
            double sellingPrice70Percent = (sellingPrice * 0.9) + sellingPrice;

            if(sellingPrice70Percent < rowPrice)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public string getSellingPrice(long? skuID)
        {
            double sellingPrice = 0;

            double summer = 0.0;

            fragrancexPrices.TryGetValue(Convert.ToInt32(skuID), out sellingPrice);
            
            if(sellingPrice == 0)
            {
                return "0.0";
            }

            double innerPrice = Convert.ToDouble(sellingPrice);

            // profit 20% by default
            summer = innerPrice + (innerPrice * 20) / 100;

            // shipping
            summer = summer + 6;

            // Amazon Fee 20%
            summer = summer + (summer * 20) / 100;

            return summer.ToString();
        }

        private bool isPriceLower(double price, double sellingPrice)
        {
            if (sellingPrice > price)
                return true;
            else
                return false;
        }
        
    }
}
