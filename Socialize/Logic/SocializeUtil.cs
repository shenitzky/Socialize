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
        public static OptinalMatchObj ConvertToOptinalMatchObj(IOptionalMatch source, int matchReqId, UserDataObj matchdDetails)
        {
            
            return new OptinalMatchObj()
            {
                Id = source.Id,
                Created = source.Created,
                MatchedFactors = source.MatchedFactors,
                MatchRequestId = matchReqId,
                MatchStrength = source.MatchStrength[matchReqId],
                MatchedDetails = matchdDetails
            };
        }

        //convert from FinalMatch to FinalMatchObj
        public static FinalMatchObj ConvertToFinalMatchObj(FinalMatch source, int matchReqId)
        {
            return new FinalMatchObj()
            {
                IsAccepted = source.IsAccepted,
                Locations = source.Locations != null ? ConvertLocationStringToLocationsList(source.Locations) : null,
                MatchStrength = source.MatchStrength != null ? source.MatchStrength[matchReqId] : 0
            };
        }

        public static string ConvertLocationsToString(List<Location> locations)
        {
            var str = locations.First().lat.ToString() + "," + locations.First().lng.ToString() + ":";
            str += locations.Last().lat.ToString() + "," + locations.Last().lng.ToString();
            return str;
        }
        public static List<Location> ConvertLocationStringToLocationsList(string locations)
        {
            var firstRawLoc = locations.Split(':')[0];
            var secRawLoc = locations.Split(':')[1];

            var firstLocObj = new Location() { lat = Double.Parse(firstRawLoc.Split(',')[0]), lng = Double.Parse(firstRawLoc.Split(',')[1]) };
            var secLocObj = new Location() { lat = Double.Parse(secRawLoc.Split(',')[0]), lng = Double.Parse(secRawLoc.Split(',')[1]) };

            return new List<Location>() { firstLocObj, secLocObj };
        }
    }
}