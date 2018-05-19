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
    }
}
