using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseModifier;
using DBTester.Models;
using OfficeOpenXml;

namespace ExcelModifier
{
    public class ShopifyExcelCreator : DBRawQueries, IExcelExtension, IDatabaseModifier
    {
        public ShopifyExcelCreator(Profile _profile, Dictionary<string, ShopifyUser> _shopifyProfile
            , Dictionary<int, Fragrancex> _fragrancex, ConcurrentDictionary<string, string> _shopifyUser
            , Dictionary<int, UPC> _upc, string _path)
        {
            profile = _profile;
            shopifyProfile = _shopifyProfile;
            fragrancex = _fragrancex;
            shopifyUser = _shopifyUser;
            upc = _upc;
            path = _path;
            dicTitle = new Dictionary<string, string>();
        }

        public ShopifyExcelCreator(Profile _profile, Dictionary<string, ShopifyUser> _shopifyProfile
            , ConcurrentDictionary<string, string> _shopifyUser, string _path)
        {
            profile = _profile;
            shopifyUser = _shopifyUser;
            shopifyProfile = _shopifyProfile;
            path = _path;
            dicTitle = new Dictionary<string, string>();
            shopifyUserTemp = new ConcurrentDictionary<string, string>();
        }

        public ConcurrentDictionary<string, string> shopifyUserTemp { get; set; }

        private Dictionary<string, ShopifyUser> shopifyProfile { get; set; }

        public ConcurrentDictionary<string, string> shopifyUser { get; set; }

        private Dictionary<int, Fragrancex> fragrancex { get; set; }

        private Dictionary<int, UPC> upc { get; set; }

        public string path { get; set; }

        private Profile profile { get; set; }

        private Dictionary<string, string> dicTitle { set; get; }
        
        public void ExcelGenerator()
        {
            // First update the database with the new items

            FileInfo file = new FileInfo(path);
            Dictionary<string, long> dicSKU = new Dictionary<string, long>();
            
            
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    listCreator(worksheet);

                    setTitleDic();

                    tableExecutor();

                    package.Save();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void saveExcel()
        {
            FileInfo file = new FileInfo(path);
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    // delete everything from the spreadsheet
                    worksheet.DeleteRow(2, worksheet.Dimension.Rows);

                    string itemID;

                    string title = "";

                    int row = 2;

                    int execption = 0;

                    setTitleDic();

                    try
                    {
                        foreach (var item in shopifyProfile
                            .OrderBy(x => x.Value.handle)
                            .OrderBy(x => x.Value.option1Value))
                        {
                            if (shopifyUser.ContainsKey(item.Value.sku + "_" + profile.ProfileUser))
                            {
                                execption++;

                                itemID = item.Value.sku;

                                Fragrancex fra = new Fragrancex();

                                fragrancex.TryGetValue(Convert.ToInt32(itemID), out fra);

                                string description = string.Empty;

                                double price = 0.0;

                                if (fra != null)
                                {
                                    description = fra.Description;

                                    price = fra.WholePriceUSD;
                                }

                                // Set Handle
                                worksheet.Cells[row, 1].Value = item.Value.handle;

                                // Logic for the title
                                title = BuildTitle(dicTitle, item.Value.handle, item.Value.collection);
                                worksheet.Cells[row, 2].Value = title;

                                //Logic for the HTML Body
                                worksheet.Cells[row, 3].Value = BuildHTML(title, row, profile.html, item.Value.image
                                    , description);

                                // Set Vendor
                                worksheet.Cells[row, 4].Value = item.Value.vendor;

                                // Set Type
                                worksheet.Cells[row, 5].Value = item.Value.type;

                                // Set Publish
                                worksheet.Cells[row, 6].Value = "TRUE";

                                // Option1 Name
                                worksheet.Cells[row, 7].Value = "Size";

                                // Option1 Value
                                worksheet.Cells[row, 8].Value = item.Value.option1Value;

                                // Set SKU
                                worksheet.Cells[row, 13].Value = item.Value.sku;

                                // Set Variant
                                worksheet.Cells[row, 14].Value = 400;

                                // Set Variant Inventory Tracker
                                worksheet.Cells[row, 15].Value = "shopify";

                                // Set Variant Inventory Policy
                                worksheet.Cells[row, 17].Value = "deny";

                                // Set Variant Fulfillment Service
                                worksheet.Cells[row, 18].Value = "manual";

                                // Set Variant Compare At Price
                                //if (item.Value.comparePrice != 0)
                                //{
                                //    worksheet.Cells[row, 20].Value = item.Value.comparePrice;
                                //}

                                // Set Variant Requires Shipping
                                worksheet.Cells[row, 21].Value = "TRUE";

                                // Set Variant Taxable
                                worksheet.Cells[row, 22].Value = "FALSE";

                                // UPC creator

                                UPC upcs = new UPC();

                                upc.TryGetValue(Convert.ToInt32(itemID), out upcs);

                                if (upcs != null)
                                {
                                    worksheet.Cells[row, 23].Value = upcs.Upc;
                                }

                                // Set Image
                                worksheet.Cells[row, 24].Value = fixPictureHTML(item.Value.image);

                                // Set Tags
                                worksheet.Cells[row, 26].Value = item.Value.tags;

                                // Set Collection
                                worksheet.Cells[row, 27].Value = item.Value.collection;

                                // Prices  

                                if (price != 0.0)
                                {
                                    // Set Price
                                    worksheet.Cells[row, 19].Value = getSellingPrice(price);
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

                                worksheet.Cells[row, 28].Value = price;

                                row++;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    package.Save();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void tableExecutor()
        {
            try
            {
                TableExecutor();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void setTitleDic()
        {
            string valueTitle = string.Empty;
            string keyTitle = string.Empty;

            foreach (var item in shopifyProfile
                        .OrderBy(x => x.Value.handle)
                        .OrderBy(x => x.Value.option1Value))
            {
                if (shopifyUser.ContainsKey(item.Value.sku + "_" + profile.ProfileUser))
                {
                    if (!dicTitle.ContainsKey(item.Value.handle))
                    {
                        keyTitle = item.Value.handle;
                        if (!string.IsNullOrEmpty(item.Value.option1Value))
                        {
                            valueTitle = valueTitle + getSize(item.Value.option1Value);
                        }
                        dicTitle.Add(keyTitle, valueTitle);
                    }
                    else
                    {
                        dicTitle.TryGetValue(item.Value.handle, out valueTitle);
                        if (string.IsNullOrEmpty(item.Value.option1Value))
                        {
                            dicTitle[item.Value.handle] = valueTitle + profile.sizeDivider + " " + getSize(item.Value.option1Value);
                        }
                    }
                    valueTitle = string.Empty;
                }
            }
        }

        private string getSellingPrice(double price)
        {
            double summer = 0.0;

            // profit
            summer = price + (price * profile.profit) / 100;

            // shipping
            summer = summer + profile.shipping;

            // fee (Amazon or eBay)
            summer = summer + (summer * 15) / 100;

            // Promoted

            summer = summer + (summer * 13) / 100;

            // MarkDown
            summer = summer + profile.markdown;
            
            return summer.ToString();
        }

        private string BuildHTML(string title, int row, string HTML, string pictures, string description)
        {
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

            if (getSize(value) != null)
            {
                // Remove Perfume and (Unisex)

                sb.Replace("Perfume", "");
                sb.Replace("perfume", "");
                sb.Replace("(Unisex)", "");
                sb.Replace("(unisex)", "");

                if ((sb.Length + value.Length + 3) > 80)
                    return sb.ToString();

                sb.Append(" ");

                sb.Append(value);

                if (string.IsNullOrEmpty(value) && value != "")
                {
                    sb.Append("Oz");
                }
            }
            else if (string.IsNullOrEmpty(value))
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
                        
                        // Remove testers and unboxed items
                        string title = worksheet.Cells[row, 1].Value.ToString();
                        if (!title.ToLower().Contains("tester") && !title.ToLower().Contains("unboxed")
                        && !title.ToLower().Contains("sample") && !title.ToLower().Contains("jivago")
                        && !title.ToLower().Contains("damaged box") && !title.ToLower().Contains("scratched box")
                        && !title.ToLower().Contains("damaged packaging"))
                        {
                            if(shopifyUser.TryAdd(worksheet.Cells[row, 13].Value?.ToString() + "_" + profile.ProfileUser, profile.ProfileUser))
                            {
                                shopifyUserTemp.TryAdd(worksheet.Cells[row, 13].Value?.ToString() + "_" + profile.ProfileUser, profile.ProfileUser);
                            }

                            if (!shopifyProfile.ContainsKey(worksheet.Cells[row, 13].Value.ToString()))
                            {
                                ShopifyUser user = new ShopifyUser();

                                user.sku = worksheet.Cells[row, 13].Value?.ToString();
                                user.handle = worksheet.Cells[row, 1].Value?.ToString();
                                user.title = worksheet.Cells[row, 2].Value?.ToString();
                                user.vendor = worksheet.Cells[row, 4].Value?.ToString();
                                user.type = worksheet.Cells[row, 5].Value?.ToString();
                                user.option1Value = worksheet.Cells[row, 8].Value?.ToString();
                                user.image = fixPictureHTML(worksheet.Cells[row, 24].Value?.ToString());
                                user.tags = worksheet.Cells[row, 26].Value?.ToString();
                                user.collection = worksheet.Cells[row, 27].Value?.ToString();
                                
                                shopifyProfile.Add(user.sku, user);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public DataTable CreateTable()
        {
            DataTable shopifyUserTable = new DataTable("Amazon");

            ColumnMaker(shopifyUserTable, "ItemID", "System.Int32");
            //ColumnMaker(shopifyUserTable, "body", "System.String");
            ColumnMaker(shopifyUserTable, "collection", "System.String");
            //ColumnMaker(shopifyUserTable, "comparePrice", "System.Double");
            ColumnMaker(shopifyUserTable, "handle", "System.String");
            ColumnMaker(shopifyUserTable, "image", "System.String");
            //ColumnMaker(shopifyUserTable, "option1Name", "System.String");
            ColumnMaker(shopifyUserTable, "option1Value", "System.String");
            //ColumnMaker(shopifyUserTable, "price", "System.Double");
            ColumnMaker(shopifyUserTable, "sku", "System.String");
            ColumnMaker(shopifyUserTable, "tags", "System.String");
            ColumnMaker(shopifyUserTable, "title", "System.String");
            ColumnMaker(shopifyUserTable, "type", "System.String");
            //ColumnMaker(shopifyUserTable, "upc", "System.Int64");
            ColumnMaker(shopifyUserTable, "vendor", "System.String");
            //ColumnMaker(shopifyUserTable, "user", "System.String");

            return shopifyUserTable;
        }

        public void TableExecutor()
        {
            DataTable uploadShopifyUser = CreateTable();
            int bulkSize = 0;
            try
            {
                foreach (var profile in shopifyProfile)
                {
                    DataRow insideRow = uploadShopifyUser.NewRow();

                    insideRow["ItemID"] = bulkSize + 1;
                    insideRow["sku"] = profile.Value.sku;
                    insideRow["handle"] = profile.Value.handle;
                    insideRow["title"] = profile.Value.title;
                    //insideRow["body"] = profile.Value.body;
                    insideRow["vendor"] = profile.Value.vendor;
                    insideRow["type"] = profile.Value.type;
                    //insideRow["option1Name"] = profile.Value.option1Name;
                    insideRow["option1Value"] = profile.Value.option1Value;
                    //insideRow["price"] = profile.Value.price;
                    //insideRow["comparePrice"] = profile.Value.comparePrice;
                    insideRow["image"] = profile.Value.image;
                    insideRow["tags"] = profile.Value.tags;
                    insideRow["collection"] = profile.Value.collection;
                    //if(profile.Value.upc != null)
                    //{
                    //    insideRow["upc"] = profile.Value.upc;
                    //}
                    //insideRow["user"] = profile.Value.userID;

                    uploadShopifyUser.Rows.Add(insideRow);
                    uploadShopifyUser.AcceptChanges();
                    bulkSize++;
                }

                upload(uploadShopifyUser, bulkSize, "dbo.ShopifyUser");
            }
            catch(Exception e)
            {
                throw e;
            }   
        }

        
    }
}