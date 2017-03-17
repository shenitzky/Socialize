using Socialize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Logic
{
    public class OptionalMatchBuilder
    {

        public IOptionalMatch CreateOptionalMatch(MatchRequest first, MatchRequest sec, Dictionary<int,int> algResult)
        {
            var factors = first.MatchReqDetails.MatchFactors
                .Union(sec.MatchReqDetails.MatchFactors)
                .ToList();

            var ids = new List<int>() { first.Id, sec.Id };

            return new OneOnOneOptionalMatch()
            {
                Id = SocializeUtil.GeneratId(),
                Created = DateTime.Now,
                MatchedFactors = factors,
                MatchRequestIds = ids,
                MatchStrength = algResult,
                //Set both match req ids status to false by default
                Status = new Dictionary<int, bool>() { { first.Id, false}, { sec.Id, false} }
            };
        }
    }
}