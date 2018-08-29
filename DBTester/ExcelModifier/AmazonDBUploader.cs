﻿using DatabaseModifier;
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
    public class AmazonDBUploader : WholesaleHelper, IExcelExtension
    {
        public AmazonDBUploader(Dictionary<string, string> _amazonItems, List<Amazon> _amazonList, List<Amazon> _amazonList2)
        {
            amazonItems = _amazonItems;
            amazonList = _amazonList;
            amazonPrintList = _amazonList2;
            listAmazonSure = new Dictionary<string, string>();
        }
        
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
                    int row = 0;

                    for (row = 1; row <= rowCount + 1; row++)
                    {
                        execption++;

                        if (row == 1)
                        {
                            worksheet.Cells[row, 1].Value = "sku";
                            worksheet.Cells[row, 2].Value = "product-id";
                            worksheet.Cells[row, 3].Value = "product-id-type";
                            worksheet.Cells[row, 4].Value = "price";
                            worksheet.Cells[row, 5].Value = "minimum-seller-allowed-price";
                            worksheet.Cells[row, 6].Value = "maximum-seller-allowed-price";
                            worksheet.Cells[row, 7].Value = "item-condition";
                            worksheet.Cells[row, 8].Value = "quantity";
                            worksheet.Cells[row, 9].Value = "add-delete";
                            worksheet.Cells[row, 10].Value = "will-ship-internationally";
                            worksheet.Cells[row, 11].Value = "expedited-shipping";
                            worksheet.Cells[row, 12].Value = "standard-plus";
                            worksheet.Cells[row, 13].Value = "item-note";
                            worksheet.Cells[row, 14].Value = "binding";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 1].Value?.ToString()))
                            {
                                // if the first row is a perfume/Cologne
                                string rowSku = worksheet.Cells[row, 1].Value.ToString();
                                long? digitSku = DigitGetter(rowSku);
                                //double rowPrice = Convert.ToDouble(worksheet.Cells[row, price].Value);
                                string asin = worksheet.Cells[row, 2].Value.ToString();

                                if (isAmazonList(asin))
                                {
                                    amazonPrintList.RemoveAll(x => x.Asin == asin);
                                }
                                else
                                {
                                    double sellingPrice = 0.0;
                                    // Add to the dictionary
                                    if (isFragrancex(digitSku))
                                    {
                                        if (!isInDB(asin))
                                        {
                                            Amazon amazon = new Amazon();
                                            amazon.Asin = asin;
                                            skuID = DigitGetter(rowSku);
                                            amazon.sku = skuID.ToString();
                                            amazon.price = Convert.ToDouble(worksheet.Cells[row, 3].Value);
                                            amazon.wholesaler = Wholesalers.Fragrancex.ToString();
                                            listAmazonSure.TryAdd(amazon.Asin, "");
                                            amazonList.Add(amazon);
                                        }
                                    }
                                    else if (isAzImporter(rowSku))
                                    {
                                        if (!isInDB(asin))
                                        {
                                            Amazon amazon = new Amazon();
                                            amazon.Asin = asin;
                                            sellingPrice = getSellingPrice();
                                            amazon.sku = azImporterSku;
                                            amazon.price = Convert.ToDouble(worksheet.Cells[row, 3].Value);
                                            amazon.wholesaler = Wholesalers.AzImporter.ToString();
                                            listAmazonSure.TryAdd(amazon.Asin, "");
                                            amazonList.Add(amazon);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    row = 2;

                    foreach (Amazon list in amazonPrintList.Where(x => x.wholesaler == Wholesalers.Fragrancex.ToString()))
                    {
                        Random rnd = new Random();
                        worksheet.Cells[row, 1].Value = list.sku + " " + rnd.Next(1, 49999);
                        worksheet.Cells[row, 2].Value = list.Asin;
                        worksheet.Cells[row, 3].Value = 1;
                        worksheet.Cells[row, 4].Value = list.price;
                        worksheet.Cells[row, 5].Value = "delete";
                        worksheet.Cells[row, 6].Value = "delete";
                        worksheet.Cells[row, 7].Value = 11;
                        worksheet.Cells[row, 8].Value = 3;
                        worksheet.Cells[row, 9].Value = "a";
                        worksheet.Cells[row, 10].Value = "n";
                        worksheet.Cells[row, 14].Value = "unknown_binding";
                        row++;
                    }

                    row = 2;

                    foreach (Amazon list in amazonPrintList.Where(x => x.wholesaler == Wholesalers.AzImporter.ToString()))
                    {
                        Random rnd = new Random();
                        worksheet.Cells[row, 1].Value = list.sku + " " + rnd.Next(1, 49999);
                        worksheet.Cells[row, 2].Value = list.Asin;
                        worksheet.Cells[row, 3].Value = 1;
                        worksheet.Cells[row, 4].Value = list.price;
                        worksheet.Cells[row, 5].Value = "delete";
                        worksheet.Cells[row, 6].Value = "delete";
                        worksheet.Cells[row, 7].Value = 11;
                        worksheet.Cells[row, 8].Value = 3;
                        worksheet.Cells[row, 9].Value = "a";
                        worksheet.Cells[row, 10].Value = "n";
                        worksheet.Cells[row, 14].Value = "unknown_binding";
                        row++;
                    }

                    // Delete unused rows 

                    worksheet.DeleteRow(amazonPrintList.Count + 2, rowCount);
                    
                    package.Save();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private bool isInDB(string asin)
        {
            if(amazonItems.ContainsKey(asin))
            {
                if(listAmazonSure.ContainsKey(asin))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (listAmazonSure.ContainsKey(asin))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool isAmazonList(string key)
        {
            if (amazonItems.ContainsKey(key))
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

            if (sellingPrice == 0)
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

        public string path { get; set; }

        public Dictionary<int, double> fragrancexPrices { get; set; }

        public Dictionary<string, string> amazonItems { get; set; }

        public List<Amazon> amazonList { get; set; }

        private List<Amazon> amazonPrintList { get; set; }

        private List<Amazon> amazonStock { get; set; }

        private Dictionary<string, string> listAmazonSure { get; set; }

    }
}