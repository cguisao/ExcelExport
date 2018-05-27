using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// Object to hold list of <see cref="FrgxPublicApiSDK.Models.Order"/>
    /// </summary>
    public class BulkOrder
    {
        /// <summary>
        /// A list of <see cref="FrgxPublicApiSDK.Models.Order"/> objects
        /// </summary>
        public List<Order> Orders { get; set; }
        /// <summary>
        /// If true, user have to specify a billing address. If false, the service will use default billing address of the account.
        /// </summary>
        public bool BillingInfoSpecified { get; set; }

        /// <summary>
        /// Returns a string with all BulkOrder information
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            var orderPart = "";

            foreach (var o in Orders)
            {
                orderPart += "\n" + o;
            }

            return "Orders : " + orderPart + "\nBilling Info Specified : " + BillingInfoSpecified + "\n";
        }
    }
}
