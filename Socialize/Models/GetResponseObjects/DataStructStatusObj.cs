using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models.GetResponseObjects
{
    public class DataStructStatusObj
    {
        public Dictionary<int, IOptionalMatch> OptionalMatches { get; set; }
        public Dictionary<int, MatchRequest> MatchRequests { get; set; }
    }
}