using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBTester.Models;
using FrgxPublicApiSDK;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace DBTester.Code
{
    public class Helper
    {
        public static IConfiguration Configuration;
        
        public static string PricePreparer(long? itemID, Dictionary<int, double> prices,
            Profile profile)
        {
            double shipping = profile.shipping;
            double fee = profile.fee;
            double profit = profile.profit;
            double markdown = profile.markdown;

            double value;

            double summer = 0.0;

            int item = Convert.ToInt32(itemID);

            if (prices.TryGetValue(item, out value))
            {
                // profit
                summer = value + (value * profit) / 100;

                // shipping
                summer = summer + shipping;

                // fee (Amazon or eBay)
                summer = summer + (summer * 15) / 100;

                // Promoted

                summer = summer + (summer * 13) / 100;

                // MarkDown
                summer = summer + markdown;
                
            }

            return summer.ToString();
        }

        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        internal static string BuildHTML(ExcelWorksheet worksheet, int row, string HTML)
        {
            int ColCount = worksheet.Dimension.Columns;

            string[] variable = new string[6];
            
            for (int col = 1; col <= ColCount; col++)
            {
                switch (col)
                {
                    case 2:
                        HTML = HTML.Replace("HTMLTitle", worksheet.Cells[row, col].Value.ToString());
                        break;
                    case 3:
                        HTML = HTML.Replace("HTMLBody", worksheet.Cells[row, col].Value.ToString());
                        break;
                    case 24:
                        var pic = worksheet.Cells[row, col].Value.ToString()
                            .Replace("http://img.fragrancex.com/images/products/SKU/small/"
                            , "http://img.fragrancex.com/images/products/SKU/large/")
                            .Replace("http", "https");
                        HTML = HTML.Replace("HTMLPicture", pic);
                        break;
                }
            }
            return HTML;
        }

        public static string filenameFinder(string fileName)
        {
            if (fileName.ToLower().Contains("phil"))
                return "Phil.html";
            else if (fileName.ToLower().Contains("lauren"))
                return "lauren.html";
            else if (fileName.ToLower().Contains("melody"))
                return "melody.html";
            else if (fileName.ToLower().Contains("cooper"))
                return "cooper.html";
            else
                return "";
        }

        internal static void tablePreparer(string path)
        {
            FileInfo file = new FileInfo(path);

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    // SDK call 
                    var listingApiClient = new FrgxListingApiClient("346c055aaefd", "a5574c546cbbc9c10509e3c277dd7c7039b24324");

                    var allProducts = listingApiClient.GetAllProducts();

                    int row = 1;

                    foreach (var product in allProducts)
                    {
                        if (row == 1)
                        {
                            worksheet.Cells[row, 1].Value = "ItemID";
                            worksheet.Cells[row, 2].Value = "BrandName";
                            worksheet.Cells[row, 3].Value = "Description";
                            worksheet.Cells[row, 4].Value = "Gender";
                            worksheet.Cells[row, 5].Value = "Instock";
                            worksheet.Cells[row, 6].Value = "LargeImageUrl";
                            worksheet.Cells[row, 7].Value = "MetricSize";
                            worksheet.Cells[row, 8].Value = "ParentCode";
                            worksheet.Cells[row, 9].Value = "ProductName";
                            worksheet.Cells[row, 10].Value = "RetailPriceUSD";
                            worksheet.Cells[row, 11].Value = "Size";
                            worksheet.Cells[row, 12].Value = "SmallImageUrl";
                            worksheet.Cells[row, 13].Value = "Type";
                            worksheet.Cells[row, 14].Value = "WholesalePriceAUD";
                            worksheet.Cells[row, 15].Value = "WholesalePriceCAD";
                            worksheet.Cells[row, 16].Value = "WholesalePriceEUR";
                            worksheet.Cells[row, 17].Value = "WholesalePriceGBP";
                            worksheet.Cells[row, 18].Value = "WholesalePriceUSD";
                            row++;
                        }
                        else
                        {
                            worksheet.Cells[row, 1].Value = Convert.ToInt32(product.ItemId);
                            worksheet.Cells[row, 2].Value = product.BrandName;
                            worksheet.Cells[row, 3].Value = product.Description;
                            worksheet.Cells[row, 4].Value = product.Gender;
                            worksheet.Cells[row, 5].Value = product.Instock;
                            worksheet.Cells[row, 6].Value = product.LargeImageUrl;
                            worksheet.Cells[row, 7].Value = product.MetricSize;
                            worksheet.Cells[row, 8].Value = product.ParentCode;
                            worksheet.Cells[row, 9].Value = product.ProductName;
                            worksheet.Cells[row, 10].Value = product.RetailPriceUSD;
                            worksheet.Cells[row, 11].Value = product.Size;
                            worksheet.Cells[row, 12].Value = product.SmallImageUrl;
                            worksheet.Cells[row, 13].Value = product.Type;
                            worksheet.Cells[row, 14].Value = product.WholesalePriceAUD;
                            worksheet.Cells[row, 15].Value = product.WholesalePriceCAD;
                            worksheet.Cells[row, 16].Value = product.WholesalePriceEUR;
                            worksheet.Cells[row, 17].Value = product.WholesalePriceGBP;
                            worksheet.Cells[row, 18].Value = product.WholesalePriceUSD;
                            row++;
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
        
        internal static void PrepareExcel(ExcelWorksheet worksheet, int min, int max)
        {
            int rowCount = worksheet.Dimension.Rows;
            string title = "";
            for (int row = 1; row <= rowCount; row++)
            {
                if (row == 1)
                {
                    continue;
                }
                // Remove testers and unboxed items
                title = worksheet.Cells[row, 1].Value.ToString();
                if (title.ToLower().Contains("tester") || title.ToLower().Contains("unboxed")
                    || title.ToLower().Contains("sample") || title.ToLower().Contains("jivago"))
                {
                    worksheet.DeleteRow(row, 1, true);
                    row--;
                    rowCount--;
                    continue;
                }

                // Remove for phil only

                //if (row != 1 && fileName.ToLower().Contains("phil"))
                //{
                //    long price = Convert.ToInt64(worksheet.Cells[row, 19].Value);
                //    if (price < 49 || price > 61)
                //    {
                //        worksheet.DeleteRow(row, 1, true);
                //        row--;
                //        rowCount--;
                //        continue;
                //    }
                //}

                if (min != 0 && max != 0)
                {
                    long price = Convert.ToInt64(worksheet.Cells[row, 19].Value);
                    if (price <= min || price > max)
                    {
                        worksheet.DeleteRow(row, 1, true);
                        row--;
                        rowCount--;
                        continue;
                    }
                }

                else if (min != 0)
                {
                    long price = Convert.ToInt64(worksheet.Cells[row, 19].Value);
                    if (price <= min)
                    {
                        worksheet.DeleteRow(row, 1, true);
                        row--;
                        rowCount--;
                        continue;
                    }
                }
                else if(max != 0)
                {
                    long price = Convert.ToInt64(worksheet.Cells[row, 19].Value);
                    if (price >= max)
                    {
                        worksheet.DeleteRow(row, 1, true);
                        row--;
                        rowCount--;
                        continue;
                    }
                }

                //if(fileName.ToLower().Contains("lauren"))
                //{
                //    long price = Convert.ToInt64(worksheet.Cells[row, 19].Value);
                //    if (price <= 61 || price > 70)
                //    {
                //        worksheet.DeleteRow(row, 1, true);
                //        row--;
                //        rowCount--;
                //        continue;
                //    }
                //}
                //if (fileName.ToLower().Contains("melody"))
                //{
                //    long price = Convert.ToInt64(worksheet.Cells[row, 19].Value);
                //    if (price > 88)
                //    {
                //        worksheet.DeleteRow(row, 1, true);
                //        row--;
                //        rowCount--;
                //        continue;
                //    }
                //}

                //if (fileName.ToLower().Contains("cooper"))
                //{
                //    long price = Convert.ToInt64(worksheet.Cells[row, 19].Value);
                //    if (price <= 53 || price > 70)
                //    {
                //        worksheet.DeleteRow(row, 1, true);
                //        row--;
                //        rowCount--;
                //        continue;
                //    }
                //}
            }
        }

        internal static string BuildTitle(Dictionary<string, string> dicTitle, string title
            , string fragranceType, Profile profile)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(shortTitle(title));

            string value;

            dicTitle.TryGetValue(title, out value);

            if (value != null)
            {
                value = removeRepeats(value);

                if ((sb.Length + value.Length + 3) > 80)
                    return sb.ToString();

                sb.Append(" ");

                sb.Append(value);

                if (value != null && value != "")
                {
                    sb.Append("Oz");
                }
            }
            else if (value == null)
            {
                value = title;
            }

            // Remove Perfume and (Unisex)

            sb.Replace("Perfume", "");
            sb.Replace("perfume", "");
            sb.Replace("(Unisex)", "");
            sb.Replace("(unisex)", "");

            // size does not go over 80 characters

            int longTitleSize = profile.LongstartTitle.Count();
            int midTitleSize = profile.MidtartTitle.Count();
            int shortTitleSize = profile.ShortstartTitle.Count();
            
            // Start of the Title

            addingTitleStart(profile, sb, longTitleSize, midTitleSize, shortTitleSize, "EDT");

            addingTitleStart(profile, sb, longTitleSize, midTitleSize, shortTitleSize, "EDC");

            addingTitleStart(profile, sb, longTitleSize, midTitleSize, shortTitleSize, "EDP");

            if(!sb.ToString().Contains("EDT") || !sb.ToString().Contains("EDC") || !sb.ToString().Contains("EDP"))
            {
                addingTitleStart(profile, sb, longTitleSize, midTitleSize, shortTitleSize, " ");
            }

            // End of title

            if (fragranceType.ToLower() == "cologne")
            {
                string forMen = " For Men";
                if(profile.endTtile.Equals("For Women/Men") && (sb.Length + forMen.Length) <= 80)
                {
                    sb.Append(forMen);
                } 
                else if(profile.endTtile.Equals("Perfume/Cologne") && (sb.Length + fragranceType.Length - 1) <= 80)
                {
                    sb.Append(" ");
                    sb.Append(fragranceType);
                }
            }
            else if (fragranceType.ToLower() == "perfume")
            {
                string forWomen = " For Women";
                if (profile.endTtile.Equals("For Women/Men") && (sb.Length + forWomen.Length) <= 80)
                {
                    sb.Append(forWomen);
                }
                else if (profile.endTtile.Equals("Perfume/Cologne") && (sb.Length + fragranceType.Length - 1) <= 80)
                {
                    sb.Append(" ");
                    sb.Append(fragranceType);
                }
            }

            // TODO: Create a random title, I am too tired to try right now.

            //if (fileName.ToLower().Contains("lauren"))
            //{
            //    if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString().Contains("EDT"))
            //    {
            //        if (sb.Length < 65)
            //            sb.Insert(0, value: "100% Authentic ");
            //        else if (sb.Length < 67)
            //            sb.Insert(0, value: "100% Genuine ");
            //        else if (sb.Length < 70)
            //            sb.Insert(0, value: "Authentic ");
            //        else if (sb.Length < 72)
            //            sb.Insert(0, value: "Genuine ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }
            //    else if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDC") != null)
            //    {
            //        if (sb.Length < 67)
            //            sb.Insert(0, value: "100% Genuine ");
            //        else if (sb.Length < 70)
            //            sb.Insert(0, value: "Authentic ");
            //        else if (sb.Length < 72)
            //            sb.Insert(0, value: "Genuine ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }

            //    else if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDP") != null)
            //    {
            //        if (sb.Length < 70)
            //            sb.Insert(0, value: "Authentic ");
            //        else if (sb.Length < 72)
            //            sb.Insert(0, value: "Genuine ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }
            //    else
            //    {
            //        if (sb.Length < 81)
            //            sb.Insert(0, value: "Genuine ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }
            //}
            //else if (fileName.ToLower().Contains("phil"))
            //{
            //    if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString().Contains("EDT"))
            //    {
            //        if (sb.Length < 66)
            //            sb.Insert(0, value: "New Authentic ");
            //        else if (sb.Length < 69)
            //            sb.Insert(0, value: "Sealed Box ");
            //        else if (sb.Length < 71) 
            //            sb.Insert(0, value: "Original ");
            //        else if (sb.Length < 73)
            //            sb.Insert(0, value: "Sealed ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }
            //    else if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDC") != null)
            //    {
            //        if (sb.Length < 69)
            //            sb.Insert(0, value: "Sealed Box ");
            //        else if (sb.Length < 71)
            //            sb.Insert(0, value: "Original ");
            //        else if (sb.Length < 73)
            //            sb.Insert(0, value: "Sealed ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }

            //    else if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDP") != null)
            //    {
            //        if (sb.Length < 71)
            //            sb.Insert(0, value: "Original ");
            //        else if (sb.Length < 73)
            //            sb.Insert(0, value: "Sealed ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }
            //    else
            //    {
            //        if (sb.Length < 73)
            //            sb.Insert(0, value: "Sealed ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }
            //}
            //else if (fileName.ToLower().Contains("melody"))
            //{
            //    // First, change cologne/perfume to for men/women

            //    sb.Replace("Cologne", "For Men");
            //    sb.Replace("Perfume", "For Women");

            //    // Second, create logic for a combination of words
            //    if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString().Contains("EDC"))
            //    {
            //        if (sb.Length < 60)
            //        {
            //            sb.Insert(0, value: "Original Sealed Box ");
            //        }
            //        else if (sb.Length < 64)
            //        {
            //            sb.Insert(0, value: "Original Sealed ");
            //        }
            //        else if (sb.Length < 66)
            //        {
            //            sb.Insert(0, value: "New Authentic ");
            //        }
            //        else if (sb.Length < 73)
            //        {
            //            sb.Insert(0, value: "Sealed ");
            //        }
            //        else
            //        {
            //            sb.Insert(0, value: "New ");
            //        }

            //    }
            //    else if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDT") != null)
            //    {
            //        if (sb.Length < 64)
            //        {
            //            sb.Insert(0, value: "Original Sealed ");
            //        }
            //        else if (sb.Length < 66)
            //        {
            //            sb.Insert(0, value: "New Authentic ");
            //        }
            //        else if (sb.Length < 73)
            //        {
            //            sb.Insert(0, value: "Sealed ");
            //        }
            //        else
            //        {
            //            sb.Insert(0, value: "New ");
            //        }
            //    }

            //    else if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDP") != null)
            //    {
            //        if (sb.Length < 66)
            //        {
            //            sb.Insert(0, value: "New Authentic ");
            //        }
            //        else if (sb.Length < 73)
            //        {
            //            sb.Insert(0, value: "Sealed ");
            //        }
            //        else
            //        {
            //            sb.Insert(0, value: "New ");
            //        }
            //    }
            //    else
            //    {
            //        if (sb.Length < 73)
            //        {
            //            sb.Insert(0, value: "Sealed ");
            //        }
            //        else
            //        {
            //            sb.Insert(0, value: "New ");
            //        }
            //    }
            //}
            //else if (fileName.ToLower().Contains("cooper"))
            //{
            //    sb.Replace("Cologne", "For Men");
            //    sb.Replace("Perfume", "For Women");

            //    if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString().Contains("EDT"))
            //    {
            //        if (sb.Length < 65)
            //            sb.Insert(0, value: "100% Authentic ");
            //        else if (sb.Length < 67)
            //            sb.Insert(0, value: "100% Genuine ");
            //        else if (sb.Length < 70)
            //            sb.Insert(0, value: "Authentic ");
            //        else if (sb.Length < 72)
            //            sb.Insert(0, value: "Genuine ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }
            //    else if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDC") != null)
            //    {
            //        if (sb.Length < 67)
            //            sb.Insert(0, value: "100% Genuine ");
            //        else if (sb.Length < 70)
            //            sb.Insert(0, value: "Authentic ");
            //        else if (sb.Length < 72)
            //            sb.Insert(0, value: "Genuine ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }

            //    else if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDP") != null)
            //    {
            //        if (sb.Length < 70)
            //            sb.Insert(0, value: "Authentic ");
            //        else if (sb.Length < 72)
            //            sb.Insert(0, value: "Genuine ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }
            //    else
            //    {
            //        if (sb.Length < 81)
            //            sb.Insert(0, value: "Genuine ");
            //        else if (sb.Length < 76)
            //            sb.Insert(0, value: "New ");
            //    }
            //}

            int count = 0;

            if (sb.Length > 80)
            {
                count++;
            }

            return sb.ToString();
        }

        private static void addingTitleStart(Profile profile, StringBuilder sb, int longTitleSize, int midTitleSize
            , int shortTitleSize, string type)
        {
            if ((sb.Length + longTitleSize) < 81 && sb.ToString().Contains(type) && !string.IsNullOrEmpty(longTitleSize.ToString())
                && !sb.ToString().Contains(profile.MidtartTitle) && !sb.ToString().Contains(profile.ShortstartTitle)
                && !sb.ToString().Contains(profile.LongstartTitle))
            {
                sb.Insert(0, value: profile.LongstartTitle + " ");
            }
            else if ((sb.Length + midTitleSize) < 81 && sb.ToString().Contains(type) && !string.IsNullOrEmpty(longTitleSize.ToString())
                && !sb.ToString().Contains(profile.MidtartTitle) && !sb.ToString().Contains(profile.ShortstartTitle)
                && !sb.ToString().Contains(profile.LongstartTitle))
            {
                sb.Insert(0, value: profile.MidtartTitle + " ");
            }
            else if ((sb.Length + shortTitleSize) < 81 && sb.ToString().Contains(type) && !string.IsNullOrEmpty(longTitleSize.ToString())
                && !sb.ToString().Contains(profile.MidtartTitle) && !sb.ToString().Contains(profile.ShortstartTitle)
                && !sb.ToString().Contains(profile.LongstartTitle))
            {
                sb.Insert(0, value: profile.ShortstartTitle + " ");
            }
        }

        private static string removeRepeats(string v)
        {
            char[] subString = v.ToArray();
            string ans = "";
            string cur = "";

            foreach (char c in subString)
            {
                if (char.ToLower(c).Equals('/'))
                {
                    if (!ans.Contains(cur))
                    {
                        ans = ans + cur + "/";
                        cur = "";
                    }
                    cur = "";
                }
                else
                {
                    cur = cur + c.ToString();
                }
            }


            ans = ans + cur;

            return ans.TrimEnd('/');
        }

        private static string shortTitle(string title)
        {
            string ans = "";
            if (title.Contains("Eau De Toilette"))
                ans = title.Replace("Eau De Toilette", "EDT");

            else if (title.Contains("Eau De Cologne"))
                ans = title.Replace("Eau De Cologne", "EDC");

            else if (title.Contains("Eau De Fraiche"))
                ans = title.Replace("Eau De Fraiche", "EDF");

            else if (title.Contains("Eau De Parfum"))
                ans = title.Replace("Eau De Parfum", "EDP");
            else
                ans = title;

            return ans;
        }

        internal static Dictionary<string, string> titleDic(ExcelWorksheet worksheet, Profile profile)
        {
            int rowCount = worksheet.Dimension.Rows;
            int ColCount = worksheet.Dimension.Columns;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string prev = null;
            string cur = null;
            string result = null;

            for (int row = 1; row <= rowCount; row++)
            {

                // for Phil only
                // TODO: Create some logic so this happens on its own

                //if (row != 1)
                //{
                //    long price = Convert.ToInt64(worksheet.Cells[row, 19].Value);
                //    if (price < 49 || price > 61)
                //    {
                //        //worksheet.DeleteRow(row, 1, true);
                //        //row--;
                //        //rowCount--;
                //        continue;
                //    }
                //}

                if (row != 1 && cur == null)
                {
                    cur = worksheet.Cells[row, 2].Value.ToString()
                        + " " + worksheet.Cells[row, 27].Value.ToString();
                    result = getSize(worksheet.Cells[row, 8].Value.ToString());
                    if (row == rowCount)
                        dic.Add(cur, result);
                    continue;
                }

                else if (row != 1 && prev == null)
                {
                    prev = worksheet.Cells[row, 2].Value.ToString()
                        + " " + worksheet.Cells[row, 27].Value.ToString();
                    if (string.Compare(prev, cur) == 0)
                        result = result + profile.sizeDivider + " " + getSize(worksheet.Cells[row, 8].Value.ToString());
                    else
                    {
                        dic.Add(cur, result);
                        cur = null;
                        prev = null;
                        row--;
                    }
                }

                else if (row != 1 && (cur != null && prev != null))
                {
                    prev = worksheet.Cells[row, 2].Value.ToString()
                        + " " + worksheet.Cells[row, 27].Value.ToString();
                    if (string.Compare(prev, cur) == 0)
                    {
                        result = result + profile.sizeDivider + " " + getSize(worksheet.Cells[row, 8].Value.ToString());
                        if (row == rowCount)
                            dic.Add(cur, result);
                    }

                    else
                    {
                        dic.Add(cur, result);
                        cur = null;
                        prev = null;
                        row--;
                    }
                }

                else if (row == 1)
                    continue;
                else
                {
                    cur = null;
                    prev = null;
                    dic.Add(cur, result);
                    row = row - 2;
                }
            }

            return dic;
        }

        private static string getSize(string v)
        {
            char[] subString = v.ToArray();
            string ans = "";

            if (!v.ToLower().Contains("oz"))
                return ans;

            foreach (char c in subString)
            {
                if (char.ToLower(c).Equals('o'))
                    break;
                ans = ans + c;
            }

            return ans;
        }
    }
}
