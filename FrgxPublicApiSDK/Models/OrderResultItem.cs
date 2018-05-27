using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// Object to hold information on a <see cref="FrgxPublicApiSDK.Models.OrderItem"/> in a <see cref="FrgxPublicApiSDK.Models.Order"/> made
    /// </summary>
    public class OrderResultItem
    {
        /// <summary>
        /// Item price
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Returns a string with all OrderResultItem information
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            return "Unit Price : " + UnitPrice + "\nName : " + Name + "\n";
        }
    }
}
