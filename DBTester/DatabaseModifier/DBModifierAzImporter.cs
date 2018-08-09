using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    public class DBModifierAzImporter : Database, IDatabaseModifier
    {
        public DBModifierAzImporter(string[] lines)
        {
            this.lines = lines;
        }

        public DataTable CreateTable()
        {
            DataTable azImporterTable = new DataTable("AzImporter");

            ColumnMaker(azImporterTable, "ItemID", "System.Int32");
            ColumnMaker(azImporterTable, "Category", "System.String");
            ColumnMaker(azImporterTable, "HTMLDescription", "System.String");
            ColumnMaker(azImporterTable, "Image1", "System.String");
            ColumnMaker(azImporterTable, "Image2", "System.String");
            ColumnMaker(azImporterTable, "Image3", "System.String");
            ColumnMaker(azImporterTable, "Image4", "System.String");
            ColumnMaker(azImporterTable, "Image5", "System.String");
            ColumnMaker(azImporterTable, "Image6", "System.String");
            ColumnMaker(azImporterTable, "Image7", "System.String");
            ColumnMaker(azImporterTable, "Image8", "System.String");
            ColumnMaker(azImporterTable, "itemName", "System.String");
            ColumnMaker(azImporterTable, "MainImage", "System.String");
            ColumnMaker(azImporterTable, "Quantity", "System.Int32");
            ColumnMaker(azImporterTable, "ShortDescription", "System.String");
            ColumnMaker(azImporterTable, "Sku", "System.String");
            ColumnMaker(azImporterTable, "Weight", "System.Int32");
            ColumnMaker(azImporterTable, "WholeSale", "System.Double");

            return azImporterTable;
        }

        private string[] lines { get; set; }

        public void TableExecutor()

        {
            //long? value = 0;

            //int bulkSize = 0;

            //Dictionary<string, string> dic = new Dictionary<string, string>();

            //DataTable uploadAzImport = CreateTable();

            //foreach (var line in lines)
            //{
            //    try
            //    {
            //        //dic.Add(lines.ItemId, product.ProductName);
            //        //if (product != null)
            //        //{
            //        //    DataRow insideRow = uploadAzImport.NewRow();

            //        //    insideRow["ItemID"] = Convert.ToInt32(product.ItemId);
            //        //    insideRow["BrandName"] = product.BrandName;
            //        //    insideRow["Description"] = product.Description;
            //        //    insideRow["Gender"] = product.Gender;
            //        //    insideRow["Instock"] = product.Instock;
            //        //    insideRow["LargeImageUrl"] = product.LargeImageUrl;
            //        //    insideRow["MetricSize"] = product.MetricSize;
            //        //    insideRow["ParentCode"] = product.ParentCode;
            //        //    insideRow["ProductName"] = product.ProductName;
            //        //    insideRow["RetailPriceUSD"] = product.RetailPriceUSD;
            //        //    insideRow["Size"] = product.Size;
            //        //    insideRow["SmallImageURL"] = product.SmallImageUrl;
            //        //    insideRow["Type"] = product.Type;
            //        //    insideRow["WholePriceAUD"] = product.WholesalePriceAUD;
            //        //    insideRow["WholePriceCAD"] = product.WholesalePriceCAD;
            //        //    insideRow["WholePriceEUR"] = product.WholesalePriceEUR;
            //        //    insideRow["WholePriceGBP"] = product.WholesalePriceGBP;
            //        //    insideRow["WholePriceUSD"] = product.WholesalePriceUSD;

            //        //    if (upc.TryGetValue(Convert.ToInt32(product.ItemId), out value))
            //        //    {
            //        //        insideRow["Upc"] = value;
            //        //    }

            //        //    insideRow["UpcItemID"] = Convert.ToInt32(product.ItemId);

            //        //    uploadAzImport.Rows.Add(insideRow);
            //        //    uploadAzImport.AcceptChanges();
            //        //    bulkSize++;
            //        //}
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}

            //upload(uploadAzImport, bulkSize, "dbo.Fragrancex");

        }
    }
}
