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
    public class DBModifierPerfumeWorldWideExcel : Database, IDatabaseModifier
    {
        public DBModifierPerfumeWorldWideExcel(string path
            , Dictionary<string, PerfumeWorldWide> _perfumeWorldWide)
        {
            this.path = path;
            this.PerfumeWorldWide = _perfumeWorldWide;
        }

        private string path { set; get; }

        public DataTable CreateTable()
        {
            DataTable PerfumeWorldWideTable = new DataTable("PerfumeWorldWide");

            ColumnMaker(PerfumeWorldWideTable, "ItemID", "System.Int32");
            ColumnMaker(PerfumeWorldWideTable, "Brand", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Cost", "System.Double");
            ColumnMaker(PerfumeWorldWideTable, "Description", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Designer", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Gender", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Image", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "MSRP", "System.Double");
            ColumnMaker(PerfumeWorldWideTable, "Set", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Size", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Type", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "Weight", "System.Double");
            ColumnMaker(PerfumeWorldWideTable, "sku", "System.String");
            ColumnMaker(PerfumeWorldWideTable, "upc", "System.Int64");
            ColumnMaker(PerfumeWorldWideTable, "isInstock", "System.Boolean");

            return PerfumeWorldWideTable;
        }

        private void DatabaseFieldSet()
        {
            FileInfo file = new FileInfo(path);

            int exception = 0;

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int code = 0, brand = 0, designer = 0, size = 0, type = 0, set = 0, msrp = 0
                        , gender = 0, description = 0, image = 0, cost = 0, weight = 0, upc = 0;

                    // Set all of the isInstock to false in the dictionary

                    PerfumeWorldWide.ToDictionary(x => x.Key, x => x.Value.isInstock = false);

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if(row == 1)
                        {
                            for(int i = 1; i <= worksheet.Dimension.Columns; i++)
                            {
                                if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("code"))
                                {
                                    code = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, 2].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("brand"))
                                {
                                    brand = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("designer"))
                                {
                                    designer = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("size"))
                                {
                                    size = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("type"))
                                {
                                    type = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("gender"))
                                {
                                    gender = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("set"))
                                {
                                    set = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("description"))
                                {
                                    description = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("image"))
                                {
                                    image = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("cost"))
                                {
                                    cost =i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("weight"))
                                {
                                    weight = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("msrp"))
                                {
                                    msrp = i;
                                }
                                else if (!string.IsNullOrEmpty(worksheet.Cells[row, i].Value.ToString().ToLower())
                                    && worksheet.Cells[row, i].Value.ToString().ToLower().Equals("upc"))
                                {
                                    upc = i;
                                }
                            }
                        }
                        else
                        {
                            exception++;
                            string sku = worksheet.Cells[row, code].Value?.ToString();
                            if (!string.IsNullOrEmpty(sku) && isInDB(sku))
                            {
                                PerfumeWorldWide.Where(x => x.Key == sku).FirstOrDefault().Value.isInstock = true;
                            }
                            else
                            {
                                PerfumeWorldWide p = new PerfumeWorldWide();
                                p.sku = sku;
                                p.Brand = worksheet.Cells[row, brand].Value?.ToString();
                                p.Designer = worksheet.Cells[row, designer].Value?.ToString();
                                p.Size = worksheet.Cells[row, size].Value?.ToString();
                                p.Type = worksheet.Cells[row, type].Value?.ToString();
                                p.Gender = worksheet.Cells[row, gender].Value?.ToString();
                                p.Set = worksheet.Cells[row, set].Value?.ToString();
                                p.Designer = worksheet.Cells[row, designer].Value?.ToString();
                                p.Image = worksheet.Cells[row, image].Value?.ToString();
                                p.Cost = Convert.ToDouble(worksheet.Cells[row, cost].Value?.ToString());
                                p.Weight = Convert.ToDouble(worksheet.Cells[row, weight].Value?.ToString());
                                p.MSRP = Convert.ToDouble(worksheet.Cells[row, msrp].Value?.ToString());
                                p.upc = Convert.ToInt64(worksheet.Cells[row, upc].Value?.ToString());
                                p.isInstock = true;

                                PerfumeWorldWide.Add(sku, p);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private bool isInDB(string sku)
        {
            if(PerfumeWorldWide.ContainsKey(sku))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void TableExecutor()
        {
            FileInfo file = new FileInfo(path);

            DataTable uploadPerfumeWorldWide = CreateTable();

            int bulkSize = 0;

            int exception = 0;

            DatabaseFieldSet();

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;

                    foreach(var item in PerfumeWorldWide)
                    {
                        exception++;

                        DataRow insideRow = uploadPerfumeWorldWide.NewRow();

                        insideRow["ItemID"] = bulkSize + 1;
                        insideRow["Brand"] = item.Value.Brand;
                        insideRow["Cost"] = item.Value.Cost;
                        insideRow["Description"] = item.Value.Description;
                        insideRow["Designer"] = item.Value.Designer;
                        insideRow["Gender"] = item.Value.Gender;
                        insideRow["Image"] = item.Value.Image;
                        insideRow["MSRP"] = item.Value.MSRP;
                        insideRow["Size"] = item.Value.Size;
                        insideRow["Set"] = item.Value.Set;
                        insideRow["Type"] = item.Value.Type;
                        insideRow["Weight"] = item.Value.Weight;
                        insideRow["sku"] = item.Value.sku;
                        insideRow["upc"] = item.Value.upc;
                        insideRow["isInstock"] = item.Value.isInstock;

                        uploadPerfumeWorldWide.Rows.Add(insideRow);
                        uploadPerfumeWorldWide.AcceptChanges();

                        bulkSize++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            upload(uploadPerfumeWorldWide, bulkSize, "dbo.PerfumeWorldWide");
        }

        private Dictionary<string, PerfumeWorldWide> PerfumeWorldWide { get; set; }
    }
}
