using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Exeptions
{
    public class MatchRequestHandlerException : SocializeExeption
    {

        public MatchRequestHandlerException(string message) : base(message)
        {

        }

        public MatchRequestHandlerException(string message, Exception innerException) : base(message, innerException) { }
    }
}