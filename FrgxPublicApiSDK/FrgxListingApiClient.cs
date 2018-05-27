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
    /// Interface for FrgxListingApiClient
    /// </summary>
    public interface IFrgxListingApiClient : IFrgxApiClient
    {
        /// <summary>
        /// Gets list of products by exact brand match
        /// </summary>
        /// <param name="brand">Brand name of the product that you want to retrieve.  It is not case sensitive.</param>
        /// <returns>A list of Product objects</returns>
        List<Product> GetProductByBrand(string brand);

        /// <summary>
        /// Gets list of products by exact brand match
        /// </summary>
        /// <param name="brand">Brand name of the product that you want to retrieve.  It is not case sensitive.</param>
        /// <param name="instockOnly">API will return in-stock product only if it is empty.  Default value is true.</param>
        /// <returns>A list of Product objects</returns>
        List<Product> GetProductByBrand(string brand, bool instockOnly);

        /// <summary>
        /// Gets list of all products.
        /// </summary>
        /// <remarks>Getting the list of all products is limited to 8 times a day.  If exceeded, you will receieve a response of error 429.</remarks>
        /// <returns>A list of Product objects</returns>
        List<Product> GetAllProducts();


        /// <summary>
        /// Gets list of products with the same value as parentCode.
        /// </summary>
        /// <remarks>Getting the list of all products is limited to 8 times a day.  If exceeded, you will receieve a response of error 429.</remarks>
        /// <param name="parentCode">Parent code of the product.  If null/empty, returns a list of all products.</param>
        /// <returns>A list of all Product objects</returns>
        List<Product> GetProductByParentCode(string parentCode);
    }
    /// <summary>
    /// API Client for requesting a list of products.
    /// </summary>
    public class FrgxListingApiClient : FrgxApiClient, IFrgxListingApiClient
    {
        /// <summary>
        /// Initializes a new listing API client for requesting a list of products.
        /// </summary>
        /// <param name="accessId">API access id that can be found at the top of the API document.</param>
        /// <param name="accessKey">API access key that can be found at the top of the API document.</param>
        public FrgxListingApiClient(string accessId, string accessKey) : base(accessId, accessKey) { }

        /// <summary>
        /// Gets list of products by exact brand match
        /// </summary>
        /// <param name="brand">Brand name of the product that you want to retrieve.  It is not case sensitive.</param>
        /// <returns>A list of Product objects</returns>
        public List<Product> GetProductByBrand(string brand)
        {
            if (string.IsNullOrEmpty(brand))
            {
                throw (new EmptyFeildException("Brand can not be empty!"));
            }

            var escapedbrand = Uri.EscapeDataString(brand);

            var brandUrl = string.Format("{0}{1}", Constants.FrgxapiBaseUrl, Constants.FrgxapiGetByBrand);

            return GetList(brandUrl + escapedbrand);
        }

        /// <summary>
        /// Gets list of products by exact brand match
        /// </summary>
        /// <param name="brand">Brand name of the product that you want to retrieve.  It is not case sensitive.</param>
        /// <param name="instockOnly">API will return in-stock product only if it is empty.  Default value is true.</param>
        /// <returns>A list of Product objects</returns>
        public List<Product> GetProductByBrand(string brand, bool instockOnly)
        {
            if (string.IsNullOrEmpty(brand))
            {
                throw (new EmptyFeildException("Brand can not be empty!"));
            }

            var escapedBrand = Uri.EscapeDataString(brand);

            var brandUrl = string.Concat(Constants.FrgxapiBaseUrl, Constants.FrgxapiGetByBrand);

            return GetList(brandUrl + escapedBrand + "?instockOnly=" + instockOnly);
        }

        /// <summary>
        /// Gets list of all products.
        /// </summary>
        /// <remarks>Getting the list of all products is limited to 8 times a day.  If exceeded, you will receieve a response of error 429.</remarks>
        /// <returns>A list of Product objects</returns>
        public List<Product> GetAllProducts()
        {
            var parentUrl = string.Concat(Constants.FrgxapiBaseUrl, Constants.FrgxapiList);

            return GetList(parentUrl);
        }

        /// <summary>
        /// Gets list of products with the same value as parentCode.
        /// </summary>
        /// <remarks>Getting the list of all products is limited to 8 times a day.  If exceeded, you will receieve a response of error 429.</remarks>
        /// <param name="parentCode">Parent code of the product. If null/empty, returns a list of all products.</param>
        /// <returns>A list of all Product objects</returns>
        public List<Product> GetProductByParentCode(string parentCode)
        {
            var parentUrl = string.Concat(Constants.FrgxapiBaseUrl, Constants.FrgxapiList);

            return string.IsNullOrEmpty(parentCode) ? GetList(parentUrl) : GetList(parentUrl + parentCode);
        }

        /// <summary>
        /// Get product information by UPC code
        /// </summary>
        /// <param name="upcCode">UPC of the product that you want to retrieve</param>
        /// <returns>A product object</returns>
        /// <exception cref="FrgxPublicApiSDK.Exceptions.EmptyFeildException">Thrown if given emtpy upcCode</exception>
        public Product GetProductByUpc(string upcCode)
        {
            if (string.IsNullOrEmpty(upcCode))
            {
                throw new EmptyFeildException("upcCode can not be empty!");
            }
            var upcUrl = string.Concat(Constants.FrgxapiBaseUrl, Constants.FrgxapiUpc);

            return GetProduct(upcUrl + upcCode);
        }

        /// <summary>
        /// Get product information by Item number
        /// </summary>
        /// <param name="id">ITEM# of the product that you want to retrieve</param>
        /// <returns>A product object</returns>
        /// <exception cref="FrgxPublicApiSDK.Exceptions.EmptyFeildException">Thrown if given emtpy id</exception>
        public Product GetProductById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new EmptyFeildException("Item Id can not be empty!");
            }

            var idUrl = string.Concat(Constants.FrgxapiBaseUrl, Constants.FrgxapiProductId);

            return GetProduct(idUrl + id);

        }

        /// <summary>
        /// Private utility method that makes call to server using the FrgxApicallHelper object and casts response into a Product object
        /// </summary>
        /// <param name="url">Url the get request is sent to</param>
        /// <returns>A product object</returns>
        /// <exception cref="FrgxPublicApiSDK.Exceptions.InvalidArgumentException">Thrown if given invald id or upcCode</exception>
        private Product GetProduct(string url)
        {
            string response = FrgxApicallHelper.GetApi(url).Result;

            //If bad ID or UPC code

            if (response.ToLower().StartsWith("{\"message\":\"") || response.ToLower().StartsWith("<!doctype"))
                throw (new InvalidArgumentException("Id or upcCode is Invalid!"));

            return ProductMapper.Map(JObject.Parse(response));
        }

        /// <summary>
        /// Helper method for creating a HTTP request.
        /// The result of the request is then parsed and returned.
        /// </summary>
        /// <param name="url">URL of the HTTP request</param>
        /// <returns>A list of Product objects</returns>
        private List<Product> GetList(string url)
        {
            var response = FrgxApicallHelper.GetApi(url).Result;

            if (response.ToLower().StartsWith("{\"message\":\"") || response.ToLower().StartsWith("<!doctype"))
                return new List<Product>();

            var jList = JArray.Parse(response);

            var prodList = new List<Product>();

            foreach (JObject jProd in jList.Children())
            {
                prodList.Add(ProductMapper.Map(jProd));
            }

            return prodList;
        }
    }
}