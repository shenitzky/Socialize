﻿using Socialize.Models;
using Socialize.Models.GetResponseObjects;
using System;
using System.Device.Location;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml.Linq;

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
            var factors = source.MatchedFactors.Select(x => string.Join(",", x.SubClasses.Select(z => z.Name).ToArray())).ToArray();

            return new OptinalMatchObj()
            {
                Id = source.Id,
                Created = source.Created,
                MatchedFactors = source.MatchedFactors,
                MatchRequestId = matchReqId,
                MatchStrength = source.MatchStrength[matchReqId],
                MatchedDetails = matchdDetails,
                RawMatchFactors = factors
            };
        }

        //Get user img url by user id
        public static string GetUserImg(string userId)
        {
            using (var db = ApplicationDbContext.Create())
            {
                var user = db.Users.FirstOrDefault(x => x.Id == userId);
                return user != null ? user.ImgUrl : null;
            }
        }

        //Convert from FinalMatch to FinalMatchObj
        public static FinalMatchObj ConvertToFinalMatchObj(FinalMatch source, int matchReqId, string myUserId)
        {
            var matchedUserId = source.UsersId.FirstOrDefault(x => x != myUserId);

            return new FinalMatchObj()
            {
                IsAccepted = source.IsAccepted,
                Locations = source.Locations != null ? ConvertLocationStringToLocationsList(source.Locations) : null,
                MatchStrength = source.MatchStrength != null ? source.MatchStrength[matchReqId] : 0,
                MyImgUrl = GetUserImg(myUserId),
                MatchedImgUrl = matchedUserId != null ? GetUserImg(matchedUserId) : null
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

        public static string RandomAvatarImg()
        {
            using(var db = ApplicationDbContext.Create())
            {
                var allImages = db.AvatarImgs.ToList();

                var randomNum = new Random().Next(0, allImages.Count);
                var img = allImages[randomNum];

                return img.ImgUrl;
                
            }
        }

        public static bool IsDateDeprecated(DateTime updated, int maxDiffMilliseconds)
        {
            var now = DateTime.Now;
            var dif = (now - updated).TotalMilliseconds;
            
            return dif > maxDiffMilliseconds;
        }

        //public static string RetrieveFormatedAddress(string lat, string lng)
        //{
        //    string baseUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
        //    string requestUri = string.Format(baseUri, lat, lng);

        //    using (WebClient wc = new WebClient())
        //    {
        //        string result = wc.DownloadString(requestUri);
        //        var xmlElm = XElement.Parse(result);
        //        var status = (from elm in xmlElm.Descendants()
        //                      where
        //                        elm.Name == "status"
        //                      select elm).FirstOrDefault();
        //        if (status.Value.ToLower() == "ok")
        //        {
        //            var res = (from elm in xmlElm.Descendants()
        //                       where
        //                        elm.Name == "formatted_address"
        //                       select elm).FirstOrDefault();
        //            requestUri = res.Value;
        //            return requestUri;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}
    }
}