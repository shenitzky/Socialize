using Socialize.Models;
using Socialize.Models.GetResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.FakeData
{
    public class FakeDataUtil
    {
        public static bool Fake = true;
        public static UserDataObj CreateFakeUserData()
        {
            return new UserDataObj()
            {
                Age = 10,
                Description = "Yossi Gay",
                FirstName = "Yossi",
                LastName = "Gay",
                Id = "1143afed-6abc-4ef4-b42e-894720979b3a",
                ImgUrl = "",
                Mail = "xxx@gmail.com",
                Premium = false,

                Factors = new FactorObj[]
                {
                    new FactorObj()
                        {
                            Class = "Sport" ,
                            SubClasses = new List<string>() { "Soccer", "Tennis"}
                        },
                    new FactorObj()
                        {
                            Class = "Gamming" ,
                            SubClasses = new List<string>() { "PS4", "XBOX"}
                        }
                }
            };
        }

        public static OptinalMatchObj CreateFakeOptionalMatch()
        {
            return new OptinalMatchObj()
            {
                Created = DateTime.Now,
                Id = 22,
                MatchedFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "XXX",
                       SubClasses = new List<string>() { "YYYY" }
                   },
                   new Factor()
                   {
                       Class = "ZZZZ",
                       SubClasses = new List<string>() { "TTTTT" }
                   }
               },
                MatchRequestId = 121,
                MatchStrength = 88,
            };
        }

        public static FinalMatchObj CreateFakeFinalMatch()
        {
            return new FinalMatchObj()
            {
                Locations = new List<Location>() { new Location() { lat = 1.2, lng = 1.1}, new Location() { lat = 5.4, lng = 5.6} },
                MatchStrength = 98
            };
                
        }

        public static FactorObj[] CreateFakeFactors()
        {
            return new FactorObj[]
                {
                    new FactorObj()
                        {
                            Class = "Sport" ,
                            SubClasses = new List<string>() { "Soccer", "Tennis"}
                        },
                    new FactorObj()
                        {
                            Class = "Gamming" ,
                            SubClasses = new List<string>() { "PS4", "XBOX"}
                        }
                };
        }
    }
}