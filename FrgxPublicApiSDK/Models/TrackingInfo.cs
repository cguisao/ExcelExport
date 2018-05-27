using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// Tracking information on an order. Each orderId will have one TrackingInfo.
    /// </summary>
    public class TrackingInfo
    {
        /// <summary>
        /// Carrier used for shipping
        /// </summary>
        public string Carrier { get; set; }
        /// <summary>
        /// Carrier service used for shipping
        /// </summary>
        public string Service { get; set; }
        /// <summary>
        /// Tracking number provided by carrier
        /// </summary>
        public string TrackingNumber { get; set; }
        /// <summary>
        /// Shipped date and time
        /// </summary>
        public string DateShipped { get; set; }
        /// <summary>
        /// List of places package has gone
        /// </summary>
        public List<TrackingEvent> TrackingRecord { get; set; }

        public override string ToString()
        {
            var trackingRecord = "";

            foreach(TrackingEvent track in TrackingRecord)
            {
                trackingRecord += track.ToString();              
            }

            return "Carrier : " + Carrier
                + "\nService : " + Service
                + "\nTrackingNumber : " + TrackingNumber
                + "\nDateShipped : " + DateShipped
                + "\n\nTrackingRecord : \n" + trackingRecord + "\n";
        }
    }
}
