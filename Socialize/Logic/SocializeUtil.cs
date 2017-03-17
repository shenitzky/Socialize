using Socialize.Models;
using Socialize.Models.GetResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Logic
{
    public class SocializeUtil
    {
        //Save the latest randomize number
        private static Random Random = new Random();
        public static int GeneratId()
        {
            return Random.Next();
        }

        //convert from Factor to FactorObj
        public static FactorObj ConvertToFactorObj(Factor source)
        {
            return new FactorObj()
            {
                Class = source.Class,
                SubClasses = source.SubClasses
            };
        }

        //convert from IoptionaMatch to OptinalMatchObj
        public static OptinalMatchObj ConvertToOptinalMatchObj(IOptionalMatch source, int matchReqId)
        {
            return new OptinalMatchObj()
            {
                Id = source.Id,
                Created = source.Created,
                MatchedFactors = source.MatchedFactors,
                MatchRequestId = matchReqId,
                MatchStrength = source.MatchStrength[matchReqId]
            };
        }
    }
}