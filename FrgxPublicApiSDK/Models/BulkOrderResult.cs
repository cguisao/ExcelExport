using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// Object to hold information on a <see cref="FrgxPublicApiSDK.Models.BulkOrder"/> made
    /// </summary>
    public class BulkOrderResult
    {
        /// <summary>
        /// Bulk order Id
        /// </summary>
        public string BulkOrderId { get; set; }
        /// <summary>
        /// Order total
        /// </summary>
        public decimal BulkOrderTotal { get; set; }
        /// <summary>
        /// List of <see cref="FrgxPublicApiSDK.Models.OrderResult"/> objects
        /// </summary>
        public List<OrderResult> OrderResults { get; set; }
        /// <summary>
        /// Result
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Returns a string with all BulkOrderResult information
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            var orderResultList = "";

            if (OrderResults == null)
                return "BulkOrderId : " + BulkOrderId + "\nTotal : " + BulkOrderTotal + "\nMessage : " + Message + "\n";

            foreach (var parts in OrderResults)
            {
                orderResultList += "\n" + parts;
            }

            return "BulkOrderId : " + BulkOrderId + "\nTotal : " + BulkOrderTotal + "\nOrder Results : " + orderResultList + "Message : " + Message + "\n";
        }
    }
}
