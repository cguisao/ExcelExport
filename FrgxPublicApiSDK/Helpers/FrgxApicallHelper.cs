using Newtonsoft.Json.Linq;
﻿using FrgxPublicApiSDK.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrgxPublicApiSDK.Exceptions;

namespace FrgxPublicApiSDK.Helpers
{
    /// <summary>
    /// Interface for FrgxApicallHelper
    /// </summary>
    internal interface IFrgxApicallHelper
    {
        /// <summary>
        /// Makes a get request to server
        /// </summary>
        /// <param name="url">Url the get request is sent to</param>
        /// <returns>Response from server as a string inside of a task object or null for bad product ID/UPC code</returns>
        Task<string> GetApi(string url);

        /// <summary>
        /// Makes a post request to the server
        /// </summary>
        /// <param name="url">Url the get request is sent to</param>
        /// <param name="content">Content included in the post request. Can be of type FormUrlEncodedContent or StringContent</param>
        /// <returns>Response from server as a string inside of a task object</returns>
        Task<string> PostApi(string url, ByteArrayContent content);

        /// <summary>
        /// Gets access token using AcessId and AccessKey passed in constructor, and add the token to default header for all future requests
        /// </summary>
        void Authenticate();
    }

    /// <summary>Medium that communicates with server
    /// <para>Used as a static field in parent abstract class, shared among all child client classes</para>
    /// </summary>
    internal class FrgxApicallHelper : IFrgxApicallHelper
    {
        public string AccessId { get; set; }
        public string AccessKey { get; set; }

        /// <summary>
        /// Client object that will  be used to send requests. Will store access token in its default header
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>Constructor</summary>
        public FrgxApicallHelper(string accessId, string accessKey)
        {
            AccessId = accessId;
            AccessKey = accessKey;
            _client = new HttpClient();
        }

        /// <summary>
        /// Makes a get request to server 
        /// <para>If response says that access is denied(most commonly from token expiring) it will automatically reauthenticate and send a second request</para>
        /// </summary>
        /// <param name="url">Url the get request is sent to</param>
        /// <returns>Response from server as a string inside of a task object or null for bad product ID/UPC code</returns>
        public async Task<string> GetApi(string url)
        {
            var response = _client.GetAsync(url);
            var resultStatusCode = response.Result.StatusCode.ToString();

            if (resultStatusCode.Equals("Unauthorized"))
            {
                Authenticate();
                response = _client.GetAsync(url);
            }

            return await response.Result.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Makes a post request to the server
        /// <para>If response says that access is denied(most commonly from token expiring) it will automatically reauthenticate and send a second request</para>
        /// </summary>
        /// <param name="url">Url the get request is sent to</param>
        /// <param name="content">Content included in the post request. Can be of type FormUrlEncodedContent or StringContent</param>
        /// <returns>Response from server as a string inside of a task object</returns>
        public async Task<string> PostApi(string url, ByteArrayContent content)
        {
            var response = _client.PostAsync(url, content);
            var resultStatusCode = response.Result.StatusCode.ToString();

            if (resultStatusCode.Equals("Unauthorized"))
            {
                Authenticate();
                response = _client.PostAsync(url, content);
            }

            return await response.Result.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Gets access token using AcessId and AccessKey passed in constructor, and add the token to default header for all future requests
        /// </summary>
        /// <exception cref="FrgxPublicApiSDK.Exceptions.BadAccessIdOrKeyException">Thrown if accessID and accessKey passed in from constructor are not valid</exception>
        public void Authenticate()
        {
            var requestContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("grant_type","apiAccessKey"),
                new KeyValuePair<string, string>("apiAccessId", AccessId),
                new KeyValuePair<string, string>("apiAccessKey", AccessKey)
            });

            var response = PostApi(Constants.FrgxapiToken, requestContent).Result;
            var jResponse = JObject.Parse(response);
            var error = jResponse.GetValue("error");

            if (error != null)
            {
                throw (new BadAccessIdOrKeyException("Bad Access Id Or Key"));
            }
            
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jResponse.GetValue("access_token"));

        }
    }
}
