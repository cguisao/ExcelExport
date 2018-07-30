using FrgxPublicApiSDK.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    public class DBModifierFragrancexAPI : DBModifierFragrancexExcel, IDatabaseModifier
    {
        public DBModifierFragrancexAPI(string path, Dictionary<int, long?> upc) : base(path, upc)
        {
            this.upc = upc;
        }

        private Dictionary<int, long?> upc { get; set; }

        public List<Product> allProducts { get; set; }

        public override void TableExecutor()

        {
            long? value = 0;

            int bulkSize = 0;

            Dictionary<string, string> dic = new Dictionary<string, string>();

            DataTable uploadFragrancex = CreateTable();

            foreach (var product in allProducts)
            {
                try
                {
                    dic.Add(product.ItemId, product.ProductName);
                    if (product != null)
                    {
                        DataRow insideRow = uploadFragrancex.NewRow();

                        insideRow["ItemID"] = Convert.ToInt32(product.ItemId);
                        insideRow["BrandName"] = product.BrandName;
                        insideRow["Description"] = product.Description;
                        insideRow["Gender"] = product.Gender;
                        insideRow["Instock"] = product.Instock;
                        insideRow["LargeImageUrl"] = product.LargeImageUrl;
                        insideRow["MetricSize"] = product.MetricSize;
                        insideRow["ParentCode"] = product.ParentCode;
                        insideRow["ProductName"] = product.ProductName;
                        insideRow["RetailPriceUSD"] = product.RetailPriceUSD;
                        insideRow["Size"] = product.Size;
                        insideRow["SmallImageURL"] = product.SmallImageUrl;
                        insideRow["Type"] = product.Type;
                        insideRow["WholePriceAUD"] = product.WholesalePriceAUD;
                        insideRow["WholePriceCAD"] = product.WholesalePriceCAD;
                        insideRow["WholePriceEUR"] = product.WholesalePriceEUR;
                        insideRow["WholePriceGBP"] = product.WholesalePriceGBP;
                        insideRow["WholePriceUSD"] = product.WholesalePriceUSD;

                        if (upc.TryGetValue(Convert.ToInt32(product.ItemId), out value))
                        {
                            insideRow["Upc"] = value;
                        }

                        insideRow["UpcItemID"] = Convert.ToInt32(product.ItemId);

                        uploadFragrancex.Rows.Add(insideRow);
                        uploadFragrancex.AcceptChanges();
                        bulkSize++;
                    }
                }
                catch
                {
                    continue;
                }
            }

            upload(uploadFragrancex, bulkSize, "dbo.Fragrancex");

        }
    }
}
