using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Socialize.Models;

namespace Socialize.Logic
{
    /*
     * Intuitive match algorithem, calculate match strength between two match requests
     */
    public class IntuitiveMatchAlg : IMatchAlg
    {
        public Dictionary<int, int> CalcOptionalMatch(MatchRequest first, MatchRequest sec)
        {
            //var res = new Dictionary<int, int>() { { first.Id, 80 }, { sec.Id, 70 } };
            //return res;
            throw new NotImplementedException();
        }
    }
}