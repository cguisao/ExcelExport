using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// Object to hold information on a <see cref="FrgxPublicApiSDK.Models.Order"/> made
    /// </summary>
    public class OrderResult
    {
        /// <summary>
        /// Order Id
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// Sum of SubTotal and ShippingCharge
        /// </summary>
        public decimal GrandTotal { get; set; }
        /// <summary>
        /// Total items price
        /// </summary>
        public decimal SubTotal { get; set; }
        /// <summary>
        /// Shipping cost
        /// </summary>
        public decimal ShippingCharge { get; set; }
        /// <summary>
        /// Result code for placing an order
        /// </summary>
        public ResultCode ResultCode { get; set; }
        /// <summary>
        /// Error or warning message returned by the API
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// List of <see cref="FrgxPublicApiSDK.Models.OrderResultItem"/> objects
        /// </summary>
        public List<OrderResultItem> OrderResultItems { get; set; }

        /// <summary>
        /// Returns a string with all OrderResult information
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            var orderResultList = "";

            if (OrderResultItems != null)
            {
                foreach (OrderResultItem a in OrderResultItems)
                {
                    orderResultList += a.ToString();
                }
            }
            return "OrderId : " + OrderId + "\nGrand Total : " + GrandTotal + "\nSub Total : " + SubTotal + "\nShipping Charge : " + ShippingCharge + "\nResult Code : " + ResultCode + "\nMessage : " + Message + "\nOrder Result Items : " + orderResultList + "\n";
        }
    }
}
