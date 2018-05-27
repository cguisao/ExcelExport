using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrgxPublicApiSDK.Models;
using Newtonsoft.Json.Linq;

namespace FrgxPublicApiSDK.Helpers
{
    public static class ProductMapper
    {
        public static Product Map(JObject obj)
        {
            return new Product
            {
                ItemId = NullCheck(obj.GetValue("ItemId")),
                Type = NullCheck(obj.GetValue("Type")),
                Size = NullCheck(obj.GetValue("Size")),
                MetricSize = NullCheck(obj.GetValue("MetricSize")),
                Instock = Convert.ToBoolean(NullCheck(obj.GetValue("Instock"))),
                RetailPriceUSD = Convert.ToDouble(NullCheck(obj.GetValue("RetailPriceUSD"))),
                WholesalePriceUSD = Convert.ToDouble(NullCheck(obj.GetValue("WholesalePriceUSD"))),
                WholesalePriceGBP = Convert.ToDouble(NullCheck(obj.GetValue("WholesalePriceGBP"))),
                WholesalePriceEUR = Convert.ToDouble(NullCheck(obj.GetValue("WholesalePriceEUR"))),
                WholesalePriceAUD = Convert.ToDouble(NullCheck(obj.GetValue("WholesalePriceAUD"))),
                WholesalePriceCAD = Convert.ToDouble(NullCheck(obj.GetValue("WholesalePriceCAD"))),
                ParentCode = NullCheck(obj.GetValue("ParentCode")),
                ProductName = NullCheck(obj.GetValue("ProductName")),
                BrandName = NullCheck(obj.GetValue("BrandName")),
                Gender = NullCheck(obj.GetValue("Gender")),
                Description = NullCheck(obj.GetValue("Description")),
                //SmallImageUrl = new Uri(NullCheck(obj.GetValue("SmallImageUrl"))),
                //LargeImageUrl = new Uri(NullCheck(obj.GetValue("LargeImageUrl")))
                SmallImageUrl = Convert.ToString(obj.GetValue("SmallImageUrl")),
                LargeImageUrl = Convert.ToString(obj.GetValue("LargeImageUrl"))
            };
        }

        private static string NullCheck(JToken token)
        {
            return token == null ? string.Empty : token.ToString();
        }
    }
}
