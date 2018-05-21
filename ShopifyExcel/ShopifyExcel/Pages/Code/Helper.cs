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

        private static string filenameFinder(string fileName)
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

        internal static string BuildTitle(Dictionary<string, string> dicTitle, string title)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(shortTitle(title));
            
            string value;

            dicTitle.TryGetValue(title, out value);

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

                if (row != 1 && cur == null)
                {
                    cur = worksheet.Cells[row, 2].Value.ToString();
                    result = getSize(worksheet.Cells[row, 8].Value.ToString());
                    if(row == rowCount)
                        dic.Add(cur, result);
                    continue;
                }

                else if (row != 1 && prev == null)
                {
                    prev = worksheet.Cells[row, 2].Value.ToString();
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
                    prev = worksheet.Cells[row, 2].Value.ToString();
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
