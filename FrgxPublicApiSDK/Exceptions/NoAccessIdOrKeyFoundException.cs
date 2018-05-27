using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrgxPublicApiSDK.Exceptions
{
    public class NoAccessIdOrKeyFoundException : Exception
    {
        public NoAccessIdOrKeyFoundException(string message) : base(message)
        {
        }
    }
}
