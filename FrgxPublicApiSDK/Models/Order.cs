using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// Order to hold information on an order
    /// </summary>
    public class Order
    {
        /// <summary>
        /// An <see cref="FrgxPublicApiSDK.Models.ShippingAddress"/> object. Information for shipping address
        /// </summary>
        public ShippingAddress ShippingAddress { get; set; }
        /// <summary>
        /// Shipping Method
        /// </summary>
        public ShippingMethod ShippingMethod { get; set; }
        /// <summary>
        /// External order Id
        /// </summary>
        public string ReferenceId { get; set; }
        /// <summary>
        /// True if dropship order
        /// </summary>
        public bool IsDropship { get; set; }
        /// <summary>
        /// True if gift wrapped
        /// </summary>
        public bool IsGiftWrapped { get; set; }
        /// <summary>
        /// Message for gift
        /// </summary>
        public string GiftWrapMessage { get; set; }
        /// <summary>
        /// A list of <see cref="FrgxPublicApiSDK.Models.OrderItem"/> object
        /// </summary>
        public List<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// Returns a string with all Order information
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            var  orderItemList = "";

            if (OrderItems == null)
                return "Shipping Address : " + ShippingAddress + "\nShipping Method : " + ShippingMethod +
                       "\nReferenceId : " + ReferenceId + "\nIs Dropship : " + IsDropship + "\nIs Gift Wrapped : " +
                       IsGiftWrapped + "\nGift Wrap Message : " + GiftWrapMessage + "\nOrder Items : " + orderItemList +
                       "\n";

            foreach (var part in OrderItems)
            {
                orderItemList += "\n" + part;
            }
            return "Shipping Address : " + ShippingAddress + "\nShipping Method : " + ShippingMethod + "\nReferenceId : " + ReferenceId + "\nIs Dropship : " + IsDropship + "\nIs Gift Wrapped : " + IsGiftWrapped + "\nGift Wrap Message : " + GiftWrapMessage + "\nOrder Items : " + orderItemList + "\n";
        }
    }
}
