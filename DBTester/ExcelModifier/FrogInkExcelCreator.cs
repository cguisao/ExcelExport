using DBTester.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelModifier
{
    public class FrogInkExcelCreator : IExcelExtension
    {
        public FrogInkExcelCreator(Dictionary<int, long?> _upc, Profile _profile)
        {
            upcs = _upc;
            profile = _profile;
            titleObjects = new MultiMapFrog<FrogList>();
        }

        public string path { get; set; }

        public Dictionary<int, double> fragrancexPrices { get; set; }

        private Dictionary<int, long?> upcs { get; set; }

        private Profile profile { get; set; }

        public Dictionary<int, string> descriptions { get; set; }

        public void ExcelGenerator()
        {
            FileInfo file = new FileInfo(path);
            Dictionary<string, long> dicSKU = new Dictionary<string, long>();
            Dictionary<string, string> dicTitle = new Dictionary<string, string>();
            int execption = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    dicTitle = titleDic(worksheet);

                    // Prepare the excel and remove whatever it needs to be removed.

                    listCreator(worksheet);

                    PrepareExcel(worksheet, profile.min, profile.max);

                    int rowCount = worksheet.Dimension.Rows;

                    int ColCount = worksheet.Dimension.Columns;
                    
                    string title = "";

                    worksheet.DeleteRow(1, rowCount);

                    int row2 = 2;

                    int innerList = 0;

                    foreach(string list in titleObjects.Keys)
                    {
                        execption++;

                        if(execption == 1)
                        {
                            // set the first row

                            worksheet.Cells[1, 1].Value = "sku";
                            worksheet.Cells[1, 2].Value = "title";
                            worksheet.Cells[1, 3].Value = "description";
                            worksheet.Cells[1, 4].Value = "quantity";
                            worksheet.Cells[1, 5].Value = "price";
                            worksheet.Cells[1, 6].Value = "images";
                            worksheet.Cells[1, 7].Value = "optionname1";
                            worksheet.Cells[1, 8].Value = "optionname2";
                            worksheet.Cells[1, 9].Value = "optionname3";
                            worksheet.Cells[1, 10].Value = "optionname4";
                            worksheet.Cells[1, 11].Value = "optionname5";
                            worksheet.Cells[1, 12].Value = "option1";
                            worksheet.Cells[1, 13].Value = "option2";
                            worksheet.Cells[1, 14].Value = "option3";
                            worksheet.Cells[1, 15].Value = "option4";
                            worksheet.Cells[1, 16].Value = "option5";
                            worksheet.Cells[1, 17].Value = "product_identifier";
                            worksheet.Cells[1, 18].Value = "product_identifier_type";
                            worksheet.Cells[1, 19].Value = "brand";
                            worksheet.Cells[1, 20].Value = "cost";
                        }
                        
                        innerList = 1;
                        foreach (FrogList inList in titleObjects[list])
                        {
                            // Title gets written once if it has variations
                            if(innerList == 1 && titleObjects[list].Count != 1)
                            {
                                // set title
                                title = BuildTitle(dicTitle, list, inList.collection);
                                worksheet.Cells[row2, 2].Value = title;
                                // set description
                                worksheet.Cells[row2, 3].Value = inList.description;
                                // set images
                                worksheet.Cells[row2, 6].Value = fixPictureHTML(inList.pictures);
                                // set option name 1
                                worksheet.Cells[row2, 7].Value = "Size";
                                // set option name 2
                                worksheet.Cells[row2, 8].Value = "Fragrance Type";
                                // set option name 3
                                worksheet.Cells[row2, 9].Value = "Brand";
                                // set product identifier type
                                worksheet.Cells[row2, 18].Value = "UPC";
                                // set brand 
                                worksheet.Cells[row2, 19].Value = inList.brand;

                                innerList++;
                                row2++;
                            }
                            if(titleObjects[list].Count != 1)
                            {
                                // set sku
                                worksheet.Cells[row2, 1].Value = inList.sku;
                                // set quantity
                                worksheet.Cells[row2, 4].Value = "Quantity";
                                // set price 
                                string price = getSellingPrice(inList.sku);

                                if (double.Parse(price) != 0.0)
                                {
                                    worksheet.Cells[row2, 5].Value = double.Parse(price);
                                    // set quantity
                                    worksheet.Cells[row2, 4].Value = 3;
                                }
                                else
                                {
                                    worksheet.Cells[row2, 5].Value = 100.0;
                                    // set quantity
                                    worksheet.Cells[row2, 4].Value = 0;
                                }
                                // set option 1
                                worksheet.Cells[row2, 12].Value = inList.size;
                                // set option 2
                                worksheet.Cells[row2, 13].Value = inList.fragranceType;
                                // set option 3
                                worksheet.Cells[row2, 14].Value = inList.brand;
                                // set product identifier 
                                long? value;

                                if (upcs.TryGetValue(Convert.ToInt32(inList.sku), out value))
                                {
                                    worksheet.Cells[row2, 17].Value = value;
                                }
                            
                                row2++;
                            }
                            // Title gets written once if it does not have variations
                            else if(titleObjects[list].Count == 1)
                            {
                                // set title
                                title = BuildTitle(dicTitle, list, inList.collection);
                                worksheet.Cells[row2, 2].Value = title;
                                // set description
                                worksheet.Cells[row2, 3].Value = inList.description;
                                // set images
                                worksheet.Cells[row2, 6].Value = fixPictureHTML(inList.pictures);
                                // set option name 1
                                worksheet.Cells[row2, 7].Value = "Size";
                                // set option name 2
                                worksheet.Cells[row2, 8].Value = "Fragrance Type";
                                // set option name 3
                                worksheet.Cells[row2, 9].Value = "Brand";
                                // set product identifier type
                                worksheet.Cells[row2, 18].Value = "UPC";
                                // set brand 
                                worksheet.Cells[row2, 19].Value = inList.brand;

                                // set sku
                                worksheet.Cells[row2, 1].Value = inList.sku;
                                // set quantity
                                worksheet.Cells[row2, 4].Value = "Quantity";
                                // set price 
                                string price = getSellingPrice(inList.sku);

                                if (double.Parse(price) != 0.0)
                                {
                                    worksheet.Cells[row2, 5].Value = double.Parse(price);
                                    // set quantity
                                    worksheet.Cells[row2, 4].Value = 3;
                                }
                                else
                                {
                                    worksheet.Cells[row2, 5].Value = 100.0;
                                    // set quantity
                                    worksheet.Cells[row2, 4].Value = 0;
                                }
                                // set option 1
                                worksheet.Cells[row2, 12].Value = inList.size;
                                // set option 2
                                worksheet.Cells[row2, 13].Value = inList.fragranceType;
                                // set option 3
                                worksheet.Cells[row2, 14].Value = inList.brand;
                                // set product identifier 
                                long? value;

                                if (upcs.TryGetValue(Convert.ToInt32(inList.sku), out value))
                                {
                                    worksheet.Cells[row2, 17].Value = value;
                                }

                                row2++;
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

        public string getSellingPrice(long? itemID)
        {
            double shipping = profile.shipping;
            double fee = profile.fee;
            double profit = profile.profit;
            double markdown = profile.markdown;

            double value;

            double summer = 0.0;

            int item = Convert.ToInt32(itemID);

            if (fragrancexPrices.TryGetValue(item, out value))
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

        private void PrepareExcel(ExcelWorksheet worksheet, int min, int max)
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
                    || !title.ToLower().Contains("sample") || title.ToLower().Contains("jivago"))
                {
                    worksheet.DeleteRow(row, 1, true);
                    row--;
                    rowCount--;
                    continue;
                }

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
                else if (max != 0)
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
            }
        }

        private string BuildTitle(Dictionary<string, string> dicTitle, string title, string fragranceType)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(shortTitle(title));

            string value;

            dicTitle.TryGetValue(title + " " + fragranceType, out value);

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

            // Start of the Title

            addingTitleStart(sb, "EDT");

            addingTitleStart(sb, "EDC");

            addingTitleStart(sb, "EDP");

            if (!sb.ToString().Contains("EDT") || !sb.ToString().Contains("EDC") || !sb.ToString().Contains("EDP"))
            {
                addingTitleStart(sb, " ");
            }

            // End of title

            if (fragranceType.ToLower() == "cologne")
            {
                string forMen = " For Men";
                if (profile.endTtile.Equals("For Women/Men") && (sb.Length + forMen.Length) <= 80)
                {
                    sb.Append(forMen);
                }
                else if (profile.endTtile.Equals("Perfume/Cologne") && (sb.Length + fragranceType.Length - 1) <= 80)
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

            int count = 0;

            if (sb.Length > 80)
            {
                count++;
            }

            return sb.ToString();
        }

        private void addingTitleStart(StringBuilder sb, string type)
        {
            // size does not go over 80 characters

            int longTitleSize = profile.LongstartTitle.Count();
            int midTitleSize = profile.MidtartTitle.Count();
            int shortTitleSize = profile.ShortstartTitle.Count();

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

        private string removeRepeats(string v)
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

        private string shortTitle(string title)
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

        private Dictionary<string, string> titleDic(ExcelWorksheet worksheet)
        {
            int rowCount = worksheet.Dimension.Rows;
            int ColCount = worksheet.Dimension.Columns;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string prev = null;
            string cur = null;
            string result = null;

            for (int row = 1; row <= rowCount; row++)
            {
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

        private void listCreator(ExcelWorksheet worksheet)
        {
            int rowCount = worksheet.Dimension.Rows;
            int ColCount = worksheet.Dimension.Columns;

            for (int row = 1; row <= rowCount; row++)
            {
                if(row != 1)
                {
                    // Remove testers and unboxed items
                    string title = worksheet.Cells[row, 1].Value.ToString();
                    if (!title.ToLower().Contains("tester") && !title.ToLower().Contains("unboxed")
                    && !title.ToLower().Contains("sample") && !title.ToLower().Contains("jivago"))
                    {
                        FrogList frogList = new FrogList();

                        frogList.description = worksheet.Cells[row, 3].Value.ToString();
                        frogList.brand = worksheet.Cells[row, 4].Value.ToString();
                        frogList.fragranceType = worksheet.Cells[row, 5].Value.ToString();
                        frogList.sku = Convert.ToInt64(worksheet.Cells[row, 13].Value);
                        frogList.price = Convert.ToDouble(worksheet.Cells[row, 19].Value.ToString());
                        frogList.pictures = fixPictureHTML(worksheet.Cells[row, 24].Value.ToString());
                        frogList.size = worksheet.Cells[row, 8].Value.ToString();
                        frogList.collection = worksheet.Cells[row, 27].Value.ToString();

                        titleObjects.Add(worksheet.Cells[row, 2].Value.ToString(), frogList);
                    }
                }
            }
        }

        private string getSize(string v)
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

        private string fixPictureHTML(string html)
        {
            string returnHTML = html.Replace("http://img.fragrancex.com/images/products/SKU/small/"
            , "http://img.fragrancex.com/images/products/SKU/large/");

            if (returnHTML.Contains("httpss"))
            {
                return returnHTML.Replace("httpss", "https");
            }
            else if (!returnHTML.Contains("https") && returnHTML.Contains("http"))
            {
                return returnHTML.Replace("http", "https");
            }

            return returnHTML;
        }

        private MultiMapFrog<FrogList> titleObjects;
    }
}
