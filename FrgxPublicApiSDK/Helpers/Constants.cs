using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrgxPublicApiSDK.Helpers
{
    /*
     * Class for constants (URLs) used in the program
     */
    internal class Constants
    {
        public static string FrgxapiToken = "https://apilisting.fragrancex.com/token";
        public static string FrgxapiBulkOrder = "https://apiordering.fragrancex.com/order/PlaceBulkOrder/";
        public static string FrgxapiBaseUrl = "https://apilisting.fragrancex.com";
        public static string FrgxapiTracking = "https://apitracking.fragrancex.com/tracking/gettrackinginfo/";
        public static string FrgxapiProductId = "/product/get/";
        public static string FrgxapiUpc = "/product/getbyupc/";
        public static string FrgxapiGetByBrand = "/product/getbybrand/";
        public static string FrgxapiList = "/product/list/";
 
        
    }
}
