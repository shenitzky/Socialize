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
            using(var db = ApplicationDbContext.Create())
            {
                var fakeImg = db.AvatarImgs.First();

                return new UserDataObj()
                {
                    Age = 10,
                    Description = "Yossi Gay",
                    FirstName = "Yossi",
                    LastName = "Gay",
                    Id = "1143afed-6abc-4ef4-b42e-894720979b3a",
                    ImgUrl = fakeImg.ImgUrl,
                    Mail = "xxx@gmail.com",
                    Premium = false,

                    Factors = new Factor[]
                {
                    new Factor()
                        {
                            Class = "Sport" ,
                            SubClasses = new List<SubClass>() { new SubClass() { Name = "Soccer" } , new SubClass() { Name = "Tennis"} }
                        },
                    new Factor()
                        {
                            Class = "Work" ,
                            SubClasses = new List<SubClass>() { new SubClass() { Name = "Prog" } , new SubClass() { Name = "Eng"} }
                        },
                    new Factor()
                        {
                            Class = "Hobbies" ,
                            SubClasses = new List<SubClass>() { new SubClass() { Name = "Baking" } , new SubClass() { Name = "Fishing"}, new SubClass() { Name = "Cleaning" } }
                        },
                    new Factor()
                        {
                            Class = "Gamming" ,
                            SubClasses = new List<SubClass>() { new SubClass() { Name = "PS4" }, new SubClass() { Name = "XBOX" }, new SubClass() { Name = "GameBoy" }, new SubClass() { Name = "Tetris" }, new SubClass() { Name = "PS3" }, new SubClass() { Name = "PS2" } }
                        }
                }
                };
            }
            
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
                       SubClasses = new List<SubClass>() { new SubClass() { Name =  "YYYY", ImgUrl = "" } }
                   },
                   new Factor()
                   {
                       Class = "ZZZZ",
                       SubClasses = new List<SubClass>() { new SubClass() { Name =  "YYYY", ImgUrl = "" } }
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

        public static Factor[] CreateFakeFactors()
        {
            var domain = HttpContext.Current.Request.Url.Authority;
            var imgUrl = $"{domain}/Content/Images/Factors/games.png";

            return new Factor[]
                {
                    new Factor()
                        {
                            Class = "Sport" ,
                            SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Soccer", ImgUrl = imgUrl },
                                new SubClass() { Name = "Tennis3", ImgUrl = imgUrl },
                                new SubClass() { Name = "Basketball", ImgUrl = imgUrl },
                                new SubClass() { Name = "Tennis1", ImgUrl = imgUrl },
                                new SubClass() { Name = "Tennis2", ImgUrl = imgUrl }
                            }
                        },
                    new Factor()
                        {
                            Class = "Work" ,
                            SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Prog", ImgUrl = imgUrl },
                                new SubClass() { Name = "Eng1", ImgUrl = imgUrl},
                                new SubClass() { Name = "Eng2", ImgUrl = imgUrl },
                                new SubClass() { Name = "Eng3", ImgUrl = imgUrl },
                                new SubClass() { Name = "Eng4", ImgUrl = imgUrl }
                            }
                        },
                    new Factor()
                        {
                            Class = "Hobbies" ,
                            SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Baking", ImgUrl = imgUrl },
                                new SubClass() { Name = "Fishing", ImgUrl = imgUrl},
                                new SubClass() { Name = "Cleaning", ImgUrl = imgUrl },
                                new SubClass() { Name = "Fishing", ImgUrl = imgUrl },
                                new SubClass() { Name = "Fishing5", ImgUrl = imgUrl }
                            }
                        },
                    new Factor()
                        {
                            Class = "Gamming" ,
                            SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "PS4", ImgUrl = imgUrl },
                                new SubClass() { Name = "XBOX", ImgUrl = imgUrl },
                                new SubClass() { Name = "GameBoy", ImgUrl = imgUrl },
                                new SubClass() { Name = "Tetris", ImgUrl = imgUrl },
                                new SubClass() { Name = "PS3", ImgUrl = imgUrl },
                                new SubClass() { Name = "PS2", ImgUrl = imgUrl }
                            }
                        }
                };
        }
    }
}