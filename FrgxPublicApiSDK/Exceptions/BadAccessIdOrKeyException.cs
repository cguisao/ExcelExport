using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrgxPublicApiSDK.Exceptions
{
    public class BadAccessIdOrKeyException : Exception
    {
        public BadAccessIdOrKeyException(string message) : base(message)
        {
        }
    }
}
