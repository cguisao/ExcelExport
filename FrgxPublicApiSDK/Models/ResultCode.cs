using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrgxPublicApiSDK.Models
{
    /// <summary>
    /// The numeric code indicating the result of placing a <see cref="FrgxPublicApiSDK.Models.BulkOrder"/>
    /// </summary>
    public enum ResultCode
    {
        Failed = 0,
        Success = 1,
        SuccessWithWarning = 2
    }
}
