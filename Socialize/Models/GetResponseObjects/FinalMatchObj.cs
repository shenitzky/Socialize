using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models.GetResponseObjects
{
    public class FinalMatchObj
    {
        public int MatchStrength { get; set; }

        public List<Location> Locations { get; set; }
    }
}