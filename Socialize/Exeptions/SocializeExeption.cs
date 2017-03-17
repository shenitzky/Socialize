using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Socialize.Exeptions
{
    public class SocializeExeption : Exception
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SocializeExeption(string message) : base(message)
        {
            Log.Debug($"SocializeExeption thrown with massage {message}");
        }

        public SocializeExeption(string message, Exception innerException): base (message, innerException)
        {
            Log.Debug($"SocializeExeption thrown with massage {message}");
        }
    }
}