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
        public AmazonExcelUpdator(string _path, Dictionary<int, Fragrancex> _fragrancex
            , Dictionary<string, AzImporter> _azImporter, Dictionary<string, bool> _blackListed
            , Dictionary<int, double> _shipping, Dictionary<string, PerfumeWorldWide> _perfumeWorldWide)
        {
            this.path = _path;
            fragrancexList = _fragrancex;
            azImporterList = _azImporter;
            blackListedList = _blackListed;
            ShippingList = _shipping;
            perfumeWorldWideList = _perfumeWorldWide;
        }
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
                    int asinCol = 0;

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
                                else if (worksheet.Cells[row, column].Value.ToString().ToLower().Contains("quantity")
                                    || worksheet.Cells[row, column].Value.ToString().ToLower().Contains("qty"))
                                {
                                    quantity = column;
                                }
                                else if (worksheet.Cells[row, column].Value.ToString().ToLower().Contains("asin"))
                                {
                                    asinCol = column;
                                }
                            }

                            worksheet.Cells[row, 1].Value = "sku";
                            worksheet.Cells[row, 2].Value = "price";
                            worksheet.Cells[row, 3].Value = "minimum-seller-allowed-price";
                            worksheet.Cells[row, 4].Value = "maximum-seller-allowed-price";
                            worksheet.Cells[row, 5].Value = "quantity";
                            worksheet.Cells[row, 6].Value = "handling-time";
                            worksheet.Cells[row, 7].Value = "fulfillment-channel";
                            worksheet.Cells[row, 8].Value = "Suggested Price";
                            worksheet.Cells[row, 9].Value = "Weight Price";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 1].Value?.ToString()))
                            {
                                // if the first row is a perfume/Cologne
                                string rowSku = worksheet.Cells[row, 1].Value.ToString();
                                int digitSku = DigitGetter(rowSku);
                                double rowPrice = Convert.ToDouble(worksheet.Cells[row, price].Value);
                                string asin = Convert.ToString(worksheet.Cells[row, asinCol].Value);
                                
                                if (isBlackListed(asin))
                                {
                                    worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                    worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Pink);
                                    worksheet.Cells[row, 5].Value = 0;
                                    worksheet.Cells[row, 8].Value = "ASIN Black Listed";
                                }
                                else if (isFragrancex(digitSku))
                                {
                                    
                                    // In-stock
                                    if(fragrancexList.ContainsKey(digitSku))
                                    {
                                        skuID = DigitGetter(rowSku);
                                        Fragrancex f = new Fragrancex();
                                        fragrancexList.TryGetValue(Convert.ToInt32(skuID), out f);
                                        fragrancex = f;
                                        double sellingPrice = getSellingPrice(fragrancex.WholePriceUSD);

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
                                        else if (isPriceTooHigh(rowPrice, sellingPrice) && sellingPrice != 0)
                                        {
                                            worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                            worksheet.Cells[row, 5].Value = 3;
                                            worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.MediumBlue);
                                            worksheet.Cells[row, 8].Value = sellingPrice;
                                        }
                                        else if (sellingPrice != 0)
                                        {
                                            worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                            worksheet.Cells[row, 5].Value = 3;
                                            worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                            worksheet.Cells[row, 8].Value = sellingPrice;
                                        }
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
                                    double sellingPrice = getSellingPrice();
                                    // In-stock
                                    if (azImporter.Quantity > 0)
                                    {
                                        // Weight is not register
                                        if (!isWeightRegister(AzImporterPriceWeight))
                                        {
                                            worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                            worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                                            worksheet.Cells[row, 8].Value = "Weight Not Register";
                                        }
                                        else
                                        {
                                            // Price too low
                                            if (isPriceLower(rowPrice, sellingPrice) && sellingPrice != 0)
                                            {
                                                worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                                worksheet.Cells[row, 5].Value = worksheet.Cells[row, quantity].Value;
                                                worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                                worksheet.Cells[row, 8].Value = sellingPrice;
                                                worksheet.Cells[row, 9].Value = azImporter.Weight;
                                            }

                                            // Price is too high 
                                            else if (isPriceTooHigh(rowPrice, sellingPrice) && sellingPrice != 0)
                                            {
                                                worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                                worksheet.Cells[row, 5].Value = 3;
                                                worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.MediumBlue);
                                                worksheet.Cells[row, 8].Value = sellingPrice;
                                                worksheet.Cells[row, 9].Value = azImporter.Weight;
                                            }

                                            else
                                            {
                                                worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                                worksheet.Cells[row, 5].Value = 3;
                                                worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                                worksheet.Cells[row, 8].Value = sellingPrice;
                                                worksheet.Cells[row, 9].Value = azImporter.Weight;
                                            }
                                        }
                                    }
                                    // Out of stock
                                    else
                                    {
                                        worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                        worksheet.Cells[row, 5].Value = 0;
                                        worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                        worksheet.Cells[row, 8].Value = sellingPrice;
                                        worksheet.Cells[row, 9].Value = azImporter.Weight;
                                    }
                                    azImporterSku = "";
                                    AzImporterPriceWeight = 0.0;
                                    AzImporterWeight = 0.0;
                                }
                                else if(isPerfumeWorldWide(rowSku))
                                {
                                    // In-stock
                                    if (perfumeWorldWideList.ContainsKey(rowSku))
                                    {
                                        PerfumeWorldWide p = new PerfumeWorldWide();
                                        perfumeWorldWideList.TryGetValue(rowSku, out p);
                                        perfumeWorldWide = p;
                                        double sellingPrice = getSellingPrice(perfumeWorldWide.Cost);
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
                                        else if (isPriceTooHigh(rowPrice, sellingPrice) && sellingPrice != 0)
                                        {
                                            worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                            worksheet.Cells[row, 5].Value = 3;
                                            worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.MediumBlue);
                                            worksheet.Cells[row, 8].Value = sellingPrice;
                                        }
                                        else if (sellingPrice != 0)
                                        {
                                            worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                            worksheet.Cells[row, 5].Value = 3;
                                            worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                            worksheet.Cells[row, 8].Value = sellingPrice;
                                        }
                                    }
                                    // Out-stock
                                    else
                                    {
                                        worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                        worksheet.Cells[row, 5].Value = 0;
                                        worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                    }
                                }
                                else
                                {
                                    worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                    worksheet.Cells[row, 5].Value = worksheet.Cells[row, quantity].Value;
                                    worksheet.Cells[row, 2].Value = worksheet.Cells[row, price].Value;
                                    worksheet.Cells[row, 5].Value = 0;
                                }

                                worksheet.Cells[row, 3].Value = "";
                                worksheet.Cells[row, 4].Value = "";
                            }
                        }
                    }

                    int start = 2;

                    worksheet.Cells[execption + start, 1].Value = "Legend";
                    start++;
                    worksheet.Cells[execption + start, 1].Value = "Out of stock";
                    worksheet.Cells[execption + start, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[execption + start, 2].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    start++;
                    worksheet.Cells[execption + start, 1].Value = "Weight not Register";
                    worksheet.Cells[execption + start, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[execption + start, 2].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                    start++;
                    worksheet.Cells[execption + start, 1].Value = "Price is too High";
                    worksheet.Cells[execption + start, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[execption + start, 2].Style.Fill.BackgroundColor.SetColor(Color.MediumBlue);
                    start++;
                    worksheet.Cells[execption + start, 1].Value = "Price is too Low";
                    worksheet.Cells[execption + start, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[execption + start, 2].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    start++; ;
                    worksheet.Cells[execption + start, 1].Value = "Price is Correct";
                    worksheet.Cells[execption + start, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[execption + start, 2].Style.Fill.BackgroundColor.SetColor(Color.Green);

                    package.Save();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private bool isBlackListed(string asin)
        {
            if(blackListedList.ContainsKey(asin))
            {
                return true;
            }
            else
            {
                return false;
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
        
        public double getSellingPrice(double innerPrice)
        {
            double summer = 0.0;

            // profit 20% by default
            summer = innerPrice + (innerPrice * 20) / 100;

            // shipping
            summer = summer + 6;

            // Amazon Fee 20%
            summer = summer + (summer * 20) / 100;

            return summer;
        }

        private bool isPriceLower(double price, double sellingPrice)
        {
            if (sellingPrice > price)
                return true;
            else
                return false;
        }

        public string getSellingPrice(long? itemID)
        {
            throw new NotImplementedException();
        }
    }
}
