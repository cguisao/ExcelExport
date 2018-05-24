using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyExcel.Pages.Code
{
    public class Helper
    {
        public static void ExcelGenerator(string sWebRootFolder)
        {
            FileInfo file = new FileInfo(sWebRootFolder);
            Dictionary<string, long> dicSKU = new Dictionary<string, long>();
            Dictionary<string, string> dicTitle = new Dictionary<string, string>();

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    // Prepare the excel and remove whatever it needs to be removed.

                    Helper.PrepareExcel(worksheet, Helper.filenameFinder(file.Name));

                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    dicSKU = Helper.UPCLoadDic();
                    dicTitle = Helper.titleDic(worksheet);
                    string SKU = "";

                    int count = 0;
                    string title = "";


                    for (int row = 1; row <= rowCount; row++)
                    {

                        if (row != 1)
                        {
                            // Logic for the title
                            title = Helper.BuildTitle(dicTitle, worksheet.Cells[row, 2].Value.ToString()
                                + " " + worksheet.Cells[row, 27].Value.ToString());
                            worksheet.Cells[row, 2].Value = title;
                            if (title.Length > 80)
                                count++;

                            //Logic for the HTML Body

                            worksheet.Cells[row, 3].Value = Helper.BuildHTML(worksheet, row, file.Name);

                            // SKU creator

                            SKU = worksheet.Cells[row, 13].Value.ToString();

                            long value;
                            if (dicSKU.TryGetValue(SKU, out value))
                                if (dicSKU[SKU] != 0)
                                    worksheet.Cells[row, 23].Value = dicSKU[SKU];

                            // This logic fixes the picture in some cases

                            worksheet.Cells[row, 24].Value =
                                      worksheet.Cells[row, 24].Value.ToString()
                                          .Replace("http://img.fragrancex.com/images/products/SKU/small/"
                                          , "http://img.fragrancex.com/images/products/SKU/large/")
                                          .Replace("http", "https");

                        }
                    }


                    //Dictionary<string, string> dicSingle = new Dictionary<string, string>();
                    //dicSingle = Helper.titleDic(worksheet);

                    package.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some error occurred while importing." + ex.Message);
            }
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

        internal static StringBuilder BuildHTML(ExcelWorksheet worksheet, int row, string fileName)
        {
            int ColCount = worksheet.Dimension.Columns;

            string[] variable = new string[6];

            for (int col = 1; col <= ColCount; col++)
            {
                switch (col)
                {
                    case 2:
                        variable[0] = worksheet.Cells[row, col].Value.ToString();
                        break;
                    case 3:
                        variable[1] = worksheet.Cells[row, col].Value.ToString();
                        break;
                    case 5:
                        variable[2] = worksheet.Cells[row, col].Value.ToString();
                        break;
                    case 24:
                        variable[3] = worksheet.Cells[row, col].Value.ToString()
                            .Replace("http://img.fragrancex.com/images/products/SKU/small/"
                            , "https://img.fragrancex.com/images/products/SKU/large/");
                        break;
                }
            }

            return HTMLBuilder(filenameFinder(fileName), variable);
        }

        public static string filenameFinder(string fileName)
        {
            if (fileName.ToLower().Contains("phil"))
                return "Phil.html";
            else if (fileName.ToLower().Contains("lauren"))
                return "lauren.html";
            else
                return null;
        }

        private static StringBuilder HTMLBuilder(string filename, string[] variable)
        {
            StringBuilder sb = new StringBuilder();
            
            if (filename == null)
                return null;

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);
            
            StreamReader file = new StreamReader(path);

            string text = File.ReadAllText(path);

            for (int i = 0; i < variable.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        text = text.Replace("HTMLTitle", variable[i]);
                        break;
                    case 1:
                        text = text.Replace("HTMLBody", variable[i]);
                        break;
                    case 3:
                        text = text.Replace("HTMLPicture", variable[i]);
                        break;
                    default:
                        break;
                }
            }

            return sb.Append(text);
        }

        internal static Dictionary<string, long> UPCLoadDic()
        {
            StringBuilder sb = new StringBuilder();

            string filename = "UPC.xlsx";

            if (filename == null)
                return null;

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);

            Dictionary<string, long> dic = new Dictionary<string, long>();

            FileInfo file = new FileInfo(path);
            
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if(row != 1)
                            dic.Add(worksheet.Cells[row, 1].Value.ToString()
                                , Convert.ToInt64(worksheet.Cells[row, 2].Value?.ToString()));
                    }
                }
            }catch (Exception ex)
            {
                return null;
            }
            return dic;
        }

        internal static void PrepareExcel(ExcelWorksheet worksheet, string fileName)
        {
            int rowCount = worksheet.Dimension.Rows;
            string title = "";
            for (int row = 1; row <= rowCount; row++)
            {
                // Remove testers and unboxed items
                title = worksheet.Cells[row, 1].Value.ToString();
                if (title.ToLower().Contains("tester") || title.ToLower().Contains("unboxed"))
                {
                    worksheet.DeleteRow(row, 1, true);
                    row--;
                    rowCount--;
                    continue;
                }

                // Remove for phil only

                if(row != 1 && fileName.ToLower().Contains("phil"))
                {
                    long price = Convert.ToInt64(worksheet.Cells[row, 19].Value);
                    if (price < 49 || price > 61)
                    {
                        worksheet.DeleteRow(row, 1, true);
                        row--;
                        rowCount--;
                        continue;
                    }
                }
            }
        }

        internal static string BuildTitle(Dictionary<string, string> dicTitle, string title)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(shortTitle(title));
            
            string value;

            dicTitle.TryGetValue(title, out value);

            if(value != null)
                value = removeRepeats(value);

            if ((sb.Length + value.Length + 3) > 80)
                return sb.ToString();

            sb.Append(" ");
            
            sb.Append(value);
            
            if(value != null)
                sb.Append("Oz");

            if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString().Contains("EDT"))
            {
                if(sb.Length < 65)
                    sb.Insert(0, value: "100% Authentic ");
                else if (sb.Length < 67)
                    sb.Insert(0, value: "100% Genuine ");
                else if (sb.Length < 70)
                    sb.Insert(0, value: "Authentic ");
                else if (sb.Length < 72)
                    sb.Insert(0, value: "Genuine ");
                else if(sb.Length < 76)
                    sb.Insert(0, value: "New ");
            }
            else if(string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDC") != null)
            {
                if (sb.Length < 67)
                    sb.Insert(0, value: "100% Genuine ");
                else if (sb.Length < 70)
                    sb.Insert(0, value: "Authentic ");
                else if (sb.Length < 72)
                    sb.Insert(0, value: "Genuine ");
                else if (sb.Length < 76)
                    sb.Insert(0, value: "New ");
            }

            else if (string.IsNullOrEmpty(sb.ToString()) || sb.ToString()?.Contains("EDP") != null)
            {
                if (sb.Length < 70)
                    sb.Insert(0, value: "Authentic ");
                else if (sb.Length < 72)
                    sb.Insert(0, value: "Genuine ");
                else if (sb.Length < 76)
                    sb.Insert(0, value: "New ");
            }
            else
            {
                if (sb.Length < 81)
                    sb.Insert(0, value: "Genuine ");
                else if (sb.Length < 76)
                    sb.Insert(0, value: "New ");
            }

            int count = 0;

            if (sb.Length > 80)
                count++;

            return sb.ToString();
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

        internal static Dictionary<string, string> titleDic(ExcelWorksheet worksheet)
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
                    if(row == rowCount)
                        dic.Add(cur, result);
                    continue;
                }

                else if (row != 1 && prev == null)
                {
                    prev = worksheet.Cells[row, 2].Value.ToString()
                        + " " + worksheet.Cells[row, 27].Value.ToString();
                    if (string.Compare(prev, cur) == 0)
                        result = result + "/" + getSize(worksheet.Cells[row, 8].Value.ToString());
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
                        result = result + "/" + getSize(worksheet.Cells[row, 8].Value.ToString());
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

            foreach(char c in subString)
            {
                if (char.ToLower(c).Equals('o'))
                    break;
                ans = ans + c;
            }
            
            return ans;
        }
    }
}
