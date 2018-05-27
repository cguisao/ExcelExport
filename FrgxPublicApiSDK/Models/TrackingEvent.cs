using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrgxPublicApiSDK.Models
{
    public class TrackingEvent
    {
        public string EventDate { get; set; }
        public string EventTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string EventDateTime { get; set; }

        public override string ToString()
        {
            return "    EventDate : " + EventDate
                + "\n    EventTime : " + EventTime
                + "\n    Location : " + Location
                + "\n    Description : " + Description
                + "\n    EventDateTime : " + EventDateTime + "\n\n";
        }
    }
}
