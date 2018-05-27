using FrgxPublicApiSDK.Helpers;
using FrgxPublicApiSDK.Models;
using FrgxPublicApiSDK.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Text;
using System.Threading.Tasks;

namespace FrgxPublicApiSDK
{
    /// <summary>
    /// Interface for FrgxOrderApiClient
    /// </summary>
    public interface IFrgxOrderApiClient : IFrgxApiClient
    {
        /// <summary>
        /// Places a bulk order.</summary>
        /// <remarks>Please do not place orders one at a time.  It is best to send multiple orders
        /// together in a batch and submit the batch a few times per day, for example, at 6am, 11am, and 2pm.
        /// The API uses your default credit card for payment.  You can add or modify your credit card from My Account > Payment section.</remarks>
        /// <param name="bulkOrder">A BulkOrder object</param>
        /// <returns>A BulkOrderResult object</returns>
        BulkOrderResult PlaceBulkOrder(BulkOrder bulkOrder);
    }
    /// <summary>
    /// API Client for placing bulk orders
    /// </summary>
    public class FrgxOrderApiClient : FrgxApiClient, IFrgxOrderApiClient
    {
        /// <summary>
        /// Initializes a new order API client for placing bulk orders.
        /// </summary>
        /// <param name="accessId">API access id that can be found at the top of the API document.</param>
        /// <param name="accessKey">API access key that can be found at the top of the API document.</param>
        public FrgxOrderApiClient(string accessId, string accessKey) : base(accessId, accessKey) { }


        
        /// <summary>
        /// Places a bulk order.</summary>
        /// <remarks>Please do not place orders one at a time.  It is best to send multiple orders
        /// together in a batch and submit the batch a few times per day, for example, at 6am, 11am, and 2pm.
        /// The API uses your default credit card for payment.  You can add or modify your credit card from My Account > Payment section.</remarks>
        /// <param name="bulkOrder">A BulkOrder object</param>
        /// <returns>A BulkOrderResult object</returns>
        /// <exception cref="FrgxPublicApiSDK.Exceptions.EmptyFeildException">Thrown if missing a feild in bulkOrder</exception>
        /// <exception cref="FrgxPublicApiSDK.Exceptions.InvalidArgumentException">Thrown if a arumgment or feild is not valid</exception>
        public BulkOrderResult PlaceBulkOrder(BulkOrder bulkOrder)
        {
            if (bulkOrder == null)
                throw new EmptyFeildException(@"Field ""BulkOrder"" field cannot be empty.");

            if(bulkOrder.Orders == null || bulkOrder.Orders.Count == 0)
                throw new EmptyFeildException(@"Field ""Orders"" cannot be empty.");

            foreach (var o in bulkOrder.Orders)
            {
                if (o.OrderItems == null || o.OrderItems.Count.Equals(0))
                    throw (new EmptyFeildException("No items found in order {\n" + o + "}\n"));
            }

            var json = JObject.FromObject(bulkOrder);
            
            var jsonStringContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            
            var response = FrgxApicallHelper.PostApi(Constants.FrgxapiBulkOrder, jsonStringContent).Result;
            
            var j = JObject.Parse(response);
            
            var bulkOrderResult = JsonConvert.DeserializeObject<BulkOrderResult>(j.ToString());
            
            return bulkOrderResult;
        }
    }
}
