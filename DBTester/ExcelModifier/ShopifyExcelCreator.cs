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
    public class ShopifyExcelCreator : IExcelExtension
    {
        public ShopifyExcelCreator(Dictionary<int, long?> upcs, Profile profile)
        {
            this.upcs = upcs;
            this.profile = profile;
            titleObjects = new MultiMapShopify<ShopifyList>();
            dicTitle = new Dictionary<string, string>();
        }

        private Dictionary<int, long?> upcs { get; set; }

        public string path { get; set; }

        public Dictionary<int, double> fragrancexPrices { get; set; }

        public IDictionary<int, string> descriptions { get; set; }

        private Profile profile { get; set; }

        public void ExcelGenerator()
        {
            FileInfo file = new FileInfo(path);
            Dictionary<string, long> dicSKU = new Dictionary<string, long>();
            
            int execption = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    
                    // Prepare the excel and remove whatever it needs to be removed.

                    PrepareExcel(worksheet, profile.min, profile.max);

                    listCreator(worksheet);

                    //dicTitle = titleDic(worksheet);
                    setTitleDic();
                    
                    // delete everything from the spreadsheet
                    worksheet.DeleteRow(2, worksheet.Dimension.Rows);

                    long? itemID;

                    string title = "";

                    int row = 2;

                    foreach (string list in titleObjects.Keys)
                    {
                        foreach (ShopifyList inList in titleObjects[list])
                        {
                            execption++;

                            itemID = inList.sku;

                            // Set Handle
                            worksheet.Cells[row, 1].Value = inList.title;

                            // Logic for the title
                            title = BuildTitle(dicTitle, inList.title, inList.collection);
                            worksheet.Cells[row, 2].Value = title;

                            //Logic for the HTML Body
                            worksheet.Cells[row, 3].Value = BuildHTML(title, row, profile.html, itemID, inList.pictures);

                            // Set Vendor
                            worksheet.Cells[row, 4].Value = inList.vendor;

                            // Set Type
                            worksheet.Cells[row, 5].Value = inList.fragranceType;

                            // Set Publish
                            worksheet.Cells[row, 6].Value = "TRUE";

                            // Option1 Name
                            worksheet.Cells[row, 7].Value = inList.option1Name;

                            // Option1 Value
                            worksheet.Cells[row, 8].Value = inList.option1Value;

                            // Option2 Name
                            //worksheet.Cells[row, 9].Value = "Fragrance Type";

                            // Option2 Value
                            //worksheet.Cells[row, 10].Value = inList.fragranceType;

                            // Option3 Name
                            //worksheet.Cells[row, 11].Value = "Brand";

                            // Option3 Value
                            //worksheet.Cells[row, 12].Value = inList.brand;

                            // Set SKU
                            worksheet.Cells[row, 13].Value = inList.sku;

                            // Set Variant
                            worksheet.Cells[row, 14].Value = 400;

                            // Set Variant Inventory Tracker
                            worksheet.Cells[row, 15].Value = "shopify";

                            // Set Variant Inventory Policy
                            worksheet.Cells[row, 17].Value = "deny";

                            // Set Variant Fulfillment Service
                            worksheet.Cells[row, 18].Value = "manual";

                            // Set Variant Compare At Price
                            if(inList.comparePrice != 0)
                            {
                                worksheet.Cells[row, 20].Value = inList.comparePrice;
                            }
                            
                            // Set Variant Requires Shipping
                            worksheet.Cells[row, 21].Value = "TRUE";

                            // Set Variant Taxable
                            worksheet.Cells[row, 22].Value = "FALSE";

                            // UPC creator
                            long? value;

                            if (upcs.TryGetValue(Convert.ToInt32(itemID), out value))
                            {
                                worksheet.Cells[row, 23].Value = value;
                            }

                            // Set Image
                            worksheet.Cells[row, 24].Value = fixPictureHTML(inList.pictures);

                            // Set Tags
                            worksheet.Cells[row, 26].Value = inList.tags;

                            // Set Collection
                            worksheet.Cells[row, 27].Value = inList.collection;
                            
                            // Prices
                            string price = getSellingPrice(itemID);

                            if (double.Parse(price) != 0.0)
                            {
                                // Set Price
                                worksheet.Cells[row, 19].Value = double.Parse(price);
                                // Set Variant Inventory Qty
                                worksheet.Cells[row, 16].Value = profile.items;
                            }
                            else
                            {
                                // Set Price
                                worksheet.Cells[row, 19].Value = 100.0;
                                // Set Variant Inventory Qty
                                worksheet.Cells[row, 16].Value = 0;
                            }

                            double actualPrice = 0.0;
                            fragrancexPrices.TryGetValue(Convert.ToInt32(itemID), out actualPrice);
                            worksheet.Cells[row, 28].Value = actualPrice;

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

        private void setTitleDic()
        {
            string valueTitle = string.Empty;
            string keyTitle = string.Empty;
            int counter = 1;

            foreach (string list in titleObjects.Keys)
            {
                foreach (ShopifyList inList in titleObjects[list])
                {
                    if(counter ==1)
                    {
                        keyTitle = list;
                        valueTitle = valueTitle + getSize(inList.size);
                        counter++;
                    }
                    else
                    {
                        valueTitle = valueTitle + profile.sizeDivider + getSize(inList.size);
                    }
                }
                dicTitle.Add(keyTitle, valueTitle);
                valueTitle = string.Empty;
                counter = 1;
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

        private string BuildHTML(string title, int row, string HTML, long? itemID, string pictures)
        {
            string description = string.Empty;

            descriptions.TryGetValue(Convert.ToInt32(itemID), out description);

            HTML = HTML.Replace("HTMLTitle",title);

            HTML = HTML.Replace("HTMLBody", description);
            
            HTML = HTML.Replace("HTMLPicture", pictures);
            
            return HTML;
        }

        private string BuildTitle(Dictionary<string, string> dicTitle, string title, string fragranceType)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(shortTitle(title));

            string value;

            dicTitle.TryGetValue(title, out value);

            if (value != null)
            {
                value = removeRepeats(value);

                // Remove Perfume and (Unisex)

                sb.Replace("Perfume", "");
                sb.Replace("perfume", "");
                sb.Replace("(Unisex)", "");
                sb.Replace("(unisex)", "");

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
                    || title.ToLower().Contains("sample") || title.ToLower().Contains("jivago"))
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

            if(returnHTML.Contains("httpss"))
            {
                return returnHTML.Replace("httpss", "https");
            }
            else if(!returnHTML.Contains("https") && returnHTML.Contains("http"))
            {
                return returnHTML.Replace("http", "https");
            }

            return returnHTML;
        }

        private void listCreator(ExcelWorksheet worksheet)
        {
            int rowCount = worksheet.Dimension.Rows;
            int ColCount = worksheet.Dimension.Columns;
            int exception = 0;
            try
            {
                for (int row = 1; row <= rowCount; row++)
                {
                    if (row != 1)
                    {
                        exception++;
                        if(exception == 31)
                        {

                        }
                        // Remove testers and unboxed items
                        string title = worksheet.Cells[row, 1].Value.ToString();
                        if (!title.ToLower().Contains("tester") && !title.ToLower().Contains("unboxed")
                        && !title.ToLower().Contains("sample") && !title.ToLower().Contains("jivago")
                        && !title.ToLower().Contains("damaged box") && !title.ToLower().Contains("scratched box")
                        && !title.ToLower().Contains("damaged packaging"))
                        {
                            ShopifyList shopifyList = new ShopifyList();

                            shopifyList.title = worksheet.Cells[row, 1].Value.ToString();
                            shopifyList.brand = worksheet.Cells[row, 4].Value.ToString();
                            shopifyList.fragranceType = worksheet.Cells[row, 5].Value.ToString();
                            shopifyList.sku = Convert.ToInt64(worksheet.Cells[row, 13].Value);
                            shopifyList.price = Convert.ToDouble(worksheet.Cells[row, 19].Value.ToString());
                            shopifyList.pictures = fixPictureHTML(worksheet.Cells[row, 24].Value.ToString());
                            shopifyList.size = worksheet.Cells[row, 8].Value.ToString();
                            shopifyList.collection = worksheet.Cells[row, 27].Value.ToString();
                            shopifyList.vendor = worksheet.Cells[row, 4].Value.ToString();
                            if (worksheet.Cells[row, 20].Value != null)
                            {
                                shopifyList.comparePrice = Convert.ToInt32(worksheet.Cells[row, 20].Value);
                            }
                            shopifyList.option1Name = worksheet.Cells[row, 7].Value.ToString();
                            shopifyList.option1Value = worksheet.Cells[row, 8].Value.ToString();
                            shopifyList.tags = worksheet.Cells[row, 26].Value.ToString();

                            titleObjects.Add(worksheet.Cells[row, 1].Value.ToString(), shopifyList);
                        }
                    }
                }
            }catch(Exception e)
            {
                throw e;
            }
        }

        private MultiMapShopify<ShopifyList> titleObjects;

        private Dictionary<string, string> dicTitle { set; get; }
    }
}