using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// The numeric code indicating shipping method
    /// </summary>
    public enum ShippingMethod
    {
        Ground = 0,
        SecondDayAir = 1,
        InternationalStandard = 3,
        InternationalPremium = 4
    }
}
