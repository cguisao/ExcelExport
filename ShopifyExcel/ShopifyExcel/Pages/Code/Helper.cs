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
        internal static StringBuilder BuildHTML(ExcelWorksheet worksheet, int row)
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

            return LaurenBuilder("Lauren.html", variable);
        }

        private static StringBuilder LaurenBuilder(string filename, string[] variable)
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

        internal static object UPCFinder()
        {
            throw new NotImplementedException();
        }
    }
}
