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

        public static string FakeUserImgUrl()
        {
            using (var db = ApplicationDbContext.Create())
            {
                var fakeImg = db.AvatarImgs.First();
                return fakeImg.ImgUrl;
            }
        }

        public static OptinalMatchObj CreateFakeOptionalMatch()
        {
            using(var db = ApplicationDbContext.Create())
            {
                var fakeImg = db.AvatarImgs.First();

                var rawFactors = FakeDataUtil.CreateFakeFactorsWithoutUrl();
                var factors = rawFactors.Length > 5 ? rawFactors.Take(5) : rawFactors;
                var desc = factors.Select(x => string.Join(",", x.SubClasses.Select(z => z.Name))).ToArray();

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

                    MatchedDetails = new UserDataObj()
                    {
                        FirstName = "Moshe",
                        LastName = "Levi",

                        Age = 23,
                        ImgUrl = fakeImg.ImgUrl,

                        //Description = desc
                    }
                };
            }
        }

        public static FinalMatchObj CreateFakeFinalMatch()
        {
            return new FinalMatchObj()
            {
                Locations = new List<Location>() { new Location() { lat = 31.8962843, lng = 34.8148716}, new Location() { lat = 31.8962844, lng = 34.8148712} },
                MatchStrength = 98,
                IsAccepted = true
            };
                
        }

        private static string CreateImgUrl(string name)
        {
            var domain = "http://socialize20170520113532.azurewebsites.net";
            var imgUrl = $"{domain}/Content/Images/Factors/{name}.png";

            return imgUrl;
        }

        public static void CreateFactors()
        {
            using(var db = ApplicationDbContext.Create())
            {
                var factors = new Factor[]
                                {
                    new Factor()
                        {
                            Class = "Sport" ,
                            SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Tennis", ImgUrl = CreateImgUrl("tennis") },
                                new SubClass() { Name = "Soccer", ImgUrl = CreateImgUrl("soccer") },
                                new SubClass() { Name = "Basketball", ImgUrl = CreateImgUrl("basketball") },
                                new SubClass() { Name = "Football", ImgUrl = CreateImgUrl("football") }
                            }
                        },
                    new Factor()
                        {
                            Class = "Hobbies" ,
                            SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Painting", ImgUrl = CreateImgUrl("painting") },
                                new SubClass() { Name = "Photography", ImgUrl = CreateImgUrl("photography")},
                                new SubClass() { Name = "Riding", ImgUrl = CreateImgUrl("riding") },
                                new SubClass() { Name = "Cooking", ImgUrl = CreateImgUrl("cooking") }
                            }
                        },
                    new Factor()
                        {
                            Class = "Gamming" ,
                            SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Xbox", ImgUrl = CreateImgUrl("xbox") },
                                new SubClass() { Name = "Wii", ImgUrl = CreateImgUrl("wii")},
                                new SubClass() { Name = "Ps", ImgUrl = CreateImgUrl("ps") },
                                new SubClass() { Name = "Pc", ImgUrl = CreateImgUrl("pc") }
                            }
                        },
                    new Factor()
                        {
                            Class = "Work" ,
                            SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Student", ImgUrl = CreateImgUrl("student") },
                                new SubClass() { Name = "Lawyer", ImgUrl = CreateImgUrl("lawyer") },
                                new SubClass() { Name = "Engineer", ImgUrl = CreateImgUrl("engineer") },
                                new SubClass() { Name = "Economist", ImgUrl = CreateImgUrl("economist") }
                            }
                        }
                    };

                db.Factors.AddRange(factors);
                db.SaveChanges();
            }
            
        }

        public static Factor[] CreateFakeFactors(bool imgUrlRequire)
        {
            var domain = "http://socialize20170520113532.azurewebsites.net";
            //var domain = HttpContext.Current.Request.Url.Authority;
            var imgUrl = imgUrlRequire ? $"http://{domain}/Content/Images/Factors/games.png" : "";

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
                                new SubClass() { Name = "Fishing2", ImgUrl = imgUrl },
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

        public static Factor[] CreateFakeFactorsWithoutUrl()
        {
            return CreateFakeFactors(false);
        }
    }
}