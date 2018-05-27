using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// Object to hold all information about a product
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 6-digit Item # ex) 401231
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// Product type ex) Eau De Toilette Spray
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Product size in ounce ex) 1.6oz
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// Product size in metric ex) 375ml
        /// </summary>
        public string MetricSize { get; set; }
        /// <summary>
        /// Availability of the product. False if out-of-stock
        /// </summary>
        public bool Instock { get; set; }
        /// <summary>
        /// Retail price
        /// </summary>
        public double RetailPriceUSD { get; set; }
        /// <summary>
        /// Wholesale price in US dollar
        /// </summary>
        public double WholesalePriceUSD { get; set; }
        /// <summary>
        /// Wholesale price in British Pound
        /// </summary>
        public double WholesalePriceGBP { get; set; }
        /// <summary>
        /// Wholesale price in Euro
        /// </summary>
        public double WholesalePriceEUR { get; set; }
        /// <summary>
        /// Wholesale price Australian dollar
        /// </summary>
        public double WholesalePriceAUD { get; set; }
        /// <summary>
        /// Wholesale price in Canadian dollar
        /// </summary>
        public double WholesalePriceCAD { get; set; }
        /// <summary>
        /// Product parent code
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        /// Product name ex) 007 by James Bond
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Brand ex) Calvin Klein
        /// </summary>
        public string BrandName { get; set; }
        /// <summary>
        /// Gender ex) Men
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// Brief description of the product
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Small sized product image url
        /// </summary>
        public String SmallImageUrl { get; set; }
        /// <summary>
        /// Large sized product image url
        /// </summary>
        public String LargeImageUrl { get; set; }

        /// <summary>
        /// Returns a string with all product information
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            return "ItemId : " + ItemId + "\nType : " + Type + "\nSize : " + Size + "\nMetricSize : " + MetricSize + "\nInstock : " + Instock + "\nRetailPriceUSD : " + RetailPriceUSD + "\nWholesalePriceUSD : " + WholesalePriceUSD + "\nWholesalePriceGBP : " + WholesalePriceGBP + "\nWholesalePriceEUR : " + WholesalePriceEUR + "\nWholesalePriceAUD : " + WholesalePriceAUD + "\nWholesalePriceCAD : " + WholesalePriceCAD + "\nParentCode : " + ParentCode + "\nProductName : " + ProductName + "\nBrandName : " + BrandName + "\nGender : " + Gender + "\nDescription : " + Description + "\nSmallImageUrl : " + SmallImageUrl + "\nLargeImageUrl : " + LargeImageUrl + "\n";
        }

    }
}
