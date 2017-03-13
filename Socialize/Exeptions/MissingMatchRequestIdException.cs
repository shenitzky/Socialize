using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Exeptions
{
    public class MissingMatchRequestIdException : SocializeExeption
    {
        public MissingMatchRequestIdException(string message) : base(message){ }

        public MissingMatchRequestIdException(string message, Exception innerException): base (message, innerException){ }
    }
}