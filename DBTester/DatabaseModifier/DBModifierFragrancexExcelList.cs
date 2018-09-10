using DBTester.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    public class DBModifierFragrancexExcelList : Database, IDatabaseModifier
    {
        public DBModifierFragrancexExcelList(string path, Dictionary<int, string> _fragranceTitle)
        {
            this.path = path;
            this.fragranceTitle = _fragranceTitle;
        }

        private Dictionary<int, string> fragranceTitle { get; set; }

        private string path { get; set; }

        public DataTable CreateTable()
        {
            DataTable fragrancexTable = new DataTable("FragrancexTitle");

            ColumnMaker(fragrancexTable, "ItemID", "System.Int32");
            ColumnMaker(fragrancexTable, "Title", "System.String");

            return fragrancexTable;
        }

        public virtual void TableExecutor()
        {
            FileInfo file = new FileInfo(path);

            DataTable uploadFragrancexTitle = CreateTable();

            int bulkSize = 0;

            int exception = 0;

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        exception++;

                        int sku = Convert.ToInt32(worksheet.Cells[row, 13].Value?.ToString());

                        string size = getSize(worksheet.Cells[row, 8].Value?.ToString());

                        string title = fixTitle(worksheet.Cells[row, 2].Value?.ToString()) 
                            + " " + size + "oz";

                        fragranceTitle.TryAdd(sku, title);
                    }
                }

                foreach(var item in fragranceTitle)
                {
                    DataRow insideRow = uploadFragrancexTitle.NewRow();

                    insideRow["ItemID"] = item.Key;
                    insideRow["Title"] = item.Value;

                    uploadFragrancexTitle.Rows.Add(insideRow);
                    uploadFragrancexTitle.AcceptChanges();

                    bulkSize++;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            upload(uploadFragrancexTitle, bulkSize, "dbo.FragrancexTitle");
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

        private string fixTitle(string title)
        {
            // Change EDC, EDT, EDP
            if(title.Contains("Eau De Toilette"))
            {
                title = title.Replace("Eau De Toilette", "EDT");
            }
            else if(title.Contains("Eau De Parfum"))
            {
                title = title.Replace("Eau De Parfum", "EDP");
            }
            else if (title.Contains("Eau De Cologne"))
            {
                title = title.Replace("Eau De Cologne", "EDC");
            }
            return title;
        }
    }
}
