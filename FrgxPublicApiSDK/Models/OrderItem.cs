using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// Items in a <see cref="FrgxPublicApiSDK.Models.Order"/>
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// 6-digit Item #
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// Number of item ordered
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Returns a string with all OrderItem information
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            return "Item Id : " + ItemId + "\nQuantity : " + Quantity + "\n";
        }
    }
}
