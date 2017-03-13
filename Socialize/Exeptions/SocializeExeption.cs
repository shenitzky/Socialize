using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Exeptions
{
    public class SocializeExeption : Exception
    {
        public SocializeExeption(string message) : base(message){ }

        public SocializeExeption(string message, Exception innerException): base (message, innerException){ }
    }
}