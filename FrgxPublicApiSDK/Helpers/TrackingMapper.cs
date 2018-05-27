using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrgxPublicApiSDK.Models;
using Newtonsoft.Json.Linq;

namespace FrgxPublicApiSDK.Helpers
{
    public class TrackingMapper
    {
        public static TrackingInfo Map(JObject obj)
        {
            var trackingInfo = new TrackingInfo
            {
                Carrier = obj.GetValue("Carrier") == null ? "" : obj.GetValue("Carrier").ToString(),
                Service = obj.GetValue("Service") == null ? "" : obj.GetValue("Service").ToString(),
                TrackingNumber = obj.GetValue("TrackingNumber") == null ? "" : obj.GetValue("TrackingNumber").ToString(),
                DateShipped = obj.GetValue("DateShipped") == null ? "" : obj.GetValue("DateShipped").ToString()
            };


            var trackingRecord = new List<TrackingEvent>();

            if (obj.GetValue("TrackingRecord") == null)
            {
                trackingInfo.TrackingRecord = new List<TrackingEvent>();

                return trackingInfo;
            }

            var jList = JArray.Parse(obj.GetValue("TrackingRecord").ToString());

            foreach (JObject item in jList)
            {
                var newEvent = new TrackingEvent
                {
                    EventDate = item.GetValue("EventDate") == null ? "" : item.GetValue("EventDate").ToString(),
                    EventTime = item.GetValue("EventTime") == null ? "" : item.GetValue("EventTime").ToString(),
                    Location = item.GetValue("Location") == null ? "" : item.GetValue("Location").ToString(),
                    Description = item.GetValue("Description") == null ? "" : item.GetValue("Description").ToString(),
                    EventDateTime = item.GetValue("EventDateTime") == null ? "" : item.GetValue("EventDateTime").ToString()
                };
                trackingRecord.Add(newEvent);
            }

            trackingInfo.TrackingRecord = trackingRecord;

            return trackingInfo;
        }
    }
}
