using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Exeptions
{
    public class MissingOptionalMatchException : SocializeExeption
    {

        public MissingOptionalMatchException(string message) : base(message) {

        }

        public MissingOptionalMatchException(string message, Exception innerException): base (message, innerException){ }
    }
}