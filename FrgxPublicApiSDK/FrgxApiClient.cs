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
    /// Interface for FrgxApiClient
    /// </summary>
    public interface IFrgxApiClient
    {

    }

    /// <summary>
    /// Superclass for the listing, ordering, and tracking API client
    /// <para>Handles the authentication and token expiration</para>
    /// </summary>
    public abstract class FrgxApiClient : IFrgxApiClient
    {
        /// <summary>
        /// Static object shared among all FrgxApiClient's children that handles communications with the server
        /// </summary>
        internal static FrgxApicallHelper FrgxApicallHelper;

        /// <summary>
        /// Constructor will set up a shared static FrgxApicallHelper and if already made update id and key
        /// </summary>
        /// <param name="accessId">Access ID sent to server when requesting an access token</param>
        /// <param name="accessKey">Access key sent to server when requesting an access token</param>
        protected FrgxApiClient(string accessId, string accessKey) 
        {
            if (FrgxApicallHelper == null)
            {
                FrgxApicallHelper = new FrgxApicallHelper(accessId, accessKey);
                FrgxApicallHelper.Authenticate();
            }
            else if (FrgxApicallHelper.AccessId != accessId || FrgxApicallHelper.AccessKey != accessKey)
            {
                FrgxApicallHelper.AccessId = accessId;
                FrgxApicallHelper.AccessKey = accessKey;
                FrgxApicallHelper.Authenticate();
            }
        }
    }
}
