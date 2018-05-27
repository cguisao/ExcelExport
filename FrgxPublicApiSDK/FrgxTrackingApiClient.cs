using FrgxPublicApiSDK.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using FrgxPublicApiSDK.Exceptions;
using FrgxPublicApiSDK.Models;

namespace FrgxPublicApiSDK
{
    /// <summary>
    /// Interface for FrgxTrackingApiClient
    /// </summary>

    public interface IFrgxTrackingApiClient : IFrgxApiClient
    {
        /// <summary>
        /// Retrieves the tracking information for a given order id.
        /// </summary>
        /// <param name="orderId">Either the FragranceX order ID or external ID that you want to retrieve the tracking information for.</param>
        /// <returns>An object of basic tracking information.</returns>
        TrackingInfo GetTracking(string orderId);
    }

    /// <summary>
    /// API client for tracking orders.
    /// </summary>
    public class FrgxTrackingApiClient : FrgxApiClient, IFrgxTrackingApiClient
    {
        //Constructors
        /// <summary>
        /// Initializes a new tracking API client for tracking orders.
        /// </summary>
        /// <param name="accessId">API access id that can be found at the top of the API document.</param>
        /// <param name="accessKey">API access key that can be found at the top of the API document.</param>
        public FrgxTrackingApiClient(string accessId, string accessKey) : base(accessId, accessKey) { }

        //Public Methods
        /// <summary>
        /// Retrieves the tracking information for a given order id.
        /// </summary>
        /// <param name="orderId">Either the FragranceX order ID or external ID that you want to retrieve the tracking information for.</param>
        /// <returns>An object of basic tracking information</returns>
        /// <exception cref="FrgxPublicApiSDK.Exceptions.EmptyFeildException">Thrown if missing order id</exception>
        /// <exception cref="FrgxPublicApiSDK.Exceptions.InvalidArgumentException">Thrown if invalid order id</exception>
        public TrackingInfo GetTracking(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                throw (new EmptyFeildException("No orderId given"));

            var response = FrgxApicallHelper.GetApi(Constants.FrgxapiTracking + orderId).Result;

            var jResponse = JObject.Parse(response);

            if (jResponse.GetValue("Carrier").ToString() == "")
                throw (new InvalidArgumentException("Invalid order id: " + orderId));

            return TrackingMapper.Map(jResponse);
        }

    }
}