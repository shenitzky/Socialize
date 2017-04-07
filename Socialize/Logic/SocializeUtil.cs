using Socialize.Models;
using Socialize.Models.GetResponseObjects;
using System;
using System.Device.Location;
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

        //Recived two different coordinates and return the distance in meters
        public static double CalculateLocationPriximity(Location matchLocationX, Location matchLocationY)
        {
            var sCoord = new GeoCoordinate(matchLocationX.lat, matchLocationX.lng);
            var eCoord = new GeoCoordinate(matchLocationY.lat, matchLocationY.lng);
            return sCoord.GetDistanceTo(eCoord);
        }

        //convert from Factor to FactorObj
        public static FactorObj ConvertToFactorObj(Factor source)
        {
            return new FactorObj()
            {
                Class = source.Class,
                SubClasses = source.SubClasses.Select(x => x.Name).ToList()
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