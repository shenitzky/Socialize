using Microsoft.AspNet.Identity;
using Socialize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Socialize.Models.PostRequestObjects;
using Socialize.Models.GetResponseObjects;
using Socialize.Logic;
using Socialize.FakeData;
using System.Web.Http.Cors;
using log4net;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Web;

namespace Socialize.Controllers
{
    public class SocializeApiController : ApiController
    {
        //add logger
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Create new match request by id
        [HttpPost]
        public async Task<int> CreateMatcReq(MatchReqDetails newMatchReq)
        {
            var parseObj = JsonConvert.SerializeObject(newMatchReq);
            Log.Debug($"POST CreateMatcReq calld with match details {parseObj}");
            if (FakeDataUtil.Fake)
                return 1;

            using (var db = ApplicationDbContext.Create())
            {
                var userId = User.Identity.GetUserId();

                var matchReq = new MatchRequest();
                matchReq.MatchReqDetails = newMatchReq;
                matchReq.MatchOwner = userId;

                var matchManager = MatchManager.GetManagerInstance();
                matchManager.CreateMatchRequest(matchReq);

                return matchReq.Id;
            }
        }

        //Check if found optional match for match request by id (loop) if not Update specific match request by id with new location 
        [HttpPost]
        public async Task<OptinalMatchObj> UpdateAndCheckMatcReq(MatchReqUpdateObj matchReqUpdate)
        {
            var parseObj = JsonConvert.SerializeObject(matchReqUpdate);
            Log.Debug($"POST UpdateAndCheckMatcReq calld with updates {parseObj}");

            if (FakeDataUtil.Fake)
                return null;

            var matchManager = MatchManager.GetManagerInstance();
            var optionalMatch = matchManager.CheckMatchRequestStatus(matchReqUpdate.matchReqId);
            var result = optionalMatch != null ? SocializeUtil.ConvertToOptinalMatchObj(optionalMatch, matchReqUpdate.matchReqId) : null;

            if (result != null)
                return result;

            Log.Debug($"Optional match not found for match req: {matchReqUpdate.matchReqId}");
            var manager = MatchManager.GetManagerInstance();
            manager.UpdateMatchRequest(matchReqUpdate.matchReqId, matchReqUpdate.location);
            Log.Debug($"Match request id: {matchReqUpdate.matchReqId} location updated");
            return result;
        }

        //Confirm optional match suggestion
        [HttpPost]
        public async Task AcceptOptionalMatch(OptionalMatchIdAndMatchReqIdObj ids)
        {
            var parseObj = JsonConvert.SerializeObject(ids);
            Log.Debug($"POST AcceptOptionalMatch calld with OptionalMatchIdAndMatchReqIdObj object {parseObj}");
            if (FakeDataUtil.Fake)
                return;

            var manager = MatchManager.GetManagerInstance();
            manager.AcceptOrDeclineOptionalMatch(ids.OptionalMatchId, ids.MatchReqId, true);
        }

        //Decline optional match suggestion
        [HttpPost]
        public async Task DeclineOptionalMatch(OptionalMatchIdAndMatchReqIdObj ids)
        {
            var parseObj = JsonConvert.SerializeObject(ids);
            Log.Debug($"POST DeclineOptionalMatch calld with id object {parseObj}");
            if (FakeDataUtil.Fake)
                return;

            var manager = MatchManager.GetManagerInstance();
            manager.AcceptOrDeclineOptionalMatch(ids.OptionalMatchId, ids.MatchReqId, false);
        }

        //Check optional match status - if confirmed by all other participants (loop)
        [HttpGet]
        public async Task<FinalMatchObj> CheckOptionalMatchStatus(int optionalMatchId, int matchReqId)
        {
            Log.Debug($"GET CheckOptionalMatchStatus calld with id {optionalMatchId}");
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeFinalMatch();

            var manager = MatchManager.GetManagerInstance();
            var finalMatch = manager.CheckOptionalMatchStatus(optionalMatchId);
            if(finalMatch != null)
            {
                manager.RemoveMatchRequest(matchReqId);
                return SocializeUtil.ConvertToFinalMatchObj(finalMatch, matchReqId);
            }
            return null;
        }

        //Update user detail by user id
        [HttpPost]
        public async Task UpdateUserData(UpdateUserObj updateUserData)
        {
            var parseObj = JsonConvert.SerializeObject(updateUserData);
            Log.Debug($"POST UpdateUserData calld with user object {parseObj}");

            if (FakeDataUtil.Fake)
                return;
            using (var db = ApplicationDbContext.Create())
            {
                var userId = User.Identity.GetUserId();
                //var userId = "1143afed-6abc-4ef4-b42e-894720979b3a";
                var user = db.Users.Include(x => x.Factors).Include(x => x.Factors.Select(z => z.SubClasses)).FirstOrDefault(x => x.Id == userId);

                if (user == null)
                    throw new Exception($"Can not find userId- {userId}");

                var newFactors = updateUserData.Data;
                var originalFactors = user.Factors ?? new List<Factor>();

                var unionFactors = originalFactors.Union(newFactors);
                var distinctFactors = unionFactors.DistinctBy(x => x.Class + x.SubClasses).ToList();

                db.Factors.AddRange(distinctFactors);
                user.Factors = distinctFactors;

                await db.SaveChangesAsync();
            }
        }

        //Get user detail by user id
        [HttpGet]
        public async Task<UserDataObj> GetUserData()
        {
            Log.Debug($"GET GetUserData calld");
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeUserData();

            throw new NotImplementedException();
        }

        //Get all the available factors for user registration
        [HttpGet]
        public async Task<Factor[]> GetAllSystemFactors()
        {
            Log.Debug($"GET GetAllSystemFactors calld");
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeFactors();

            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<List<int>> Test()
        {

            return null;
            //var firstReq = new MatchReqDetails()
            //{
            //    Location = new Location() { lat = 1.1, lng = 0.1 },
            //    MatchFactors = new List<Factor>()
            //   {
            //       new Factor()
            //       {
            //           Class = "XXX",
            //           SubClasses = new List<string>() { "YYYY" }
            //       },
            //       new Factor()
            //       {
            //           Class = "ZZZZ",
            //           SubClasses = new List<string>() { "TTTTT" }
            //       }
            //   }
            //};
            //var secReq = new MatchReqDetails()
            //{
            //    Location = new Location() { lat = 1.1, lng = 0.1 },
            //    MatchFactors = new List<Factor>()
            //   {
            //       new Factor()
            //       {
            //           Class = "XXX",
            //           SubClasses = new List<string>() { "YYYY" }
            //       },
            //       new Factor()
            //       {
            //           Class = "ZZZZ",
            //           SubClasses = new List<string>() { "TTTTT" }
            //       }
            //   }
            //};

            //var firstId = await CreateMatcReq(firstReq);
            //var secId = await CreateMatcReq(secReq);

            //return new List<int>() { firstId, secId };
        }


        [HttpGet]
        public async Task Test2(int first, int sec)
        {
            var manager = MatchManager.GetManagerInstance();
            var result = new Dictionary<int, int>() { { first, 88 }, { sec, 92 } };
            //Do not use, change to OnOptionalMatchFound which is an event func
            //manager.OptionalMatchFound(result);

        }

        [HttpGet]
        public async Task AddAvatarImg()
        {
            using (var db = ApplicationDbContext.Create())
            {
                var all = db.AvatarImgs.Select(x => x);

                db.AvatarImgs.RemoveRange(all);

                var domain = HttpContext.Current.Request.Url.Authority;
                var imgUrl = $"{domain}/Content/Images/Profiles/profile";

                for (var i = 1; i <= 7; i++)
                {
                    var imageToiInsert = new AvatarImg() { ImgUrl = imgUrl + i + ".png" };
                    db.AvatarImgs.Add(imageToiInsert);
                }

                await db.SaveChangesAsync();

            }
        }

        [HttpGet]
        public async Task Test4()
        {
            MatchManager manager = MatchManager.GetManagerInstance();
            MatchReqHandler handler = MatchReqHandler.GetMatchReqHandlerInstance(AlgorithemsTypes.IntuitiveMatchAlg);

            //new Factor[]
            //    {
            //        new Factor()
            //            {
            //                Class = "Sport" ,
            //                SubClasses = new List<SubClass>()
            //                {
            //                    new SubClass() { Name = "Soccer", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Tennis3", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Basketball", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Tennis1", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Tennis2", ImgUrl = imgUrl }
            //                }
            //            },
            //        new Factor()
            //            {
            //                Class = "Work" ,
            //                SubClasses = new List<SubClass>()
            //                {
            //                    new SubClass() { Name = "Prog", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Eng1", ImgUrl = imgUrl},
            //                    new SubClass() { Name = "Eng2", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Eng3", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Eng4", ImgUrl = imgUrl }
            //                }
            //            },
            //        new Factor()
            //            {
            //                Class = "Hobbies" ,
            //                SubClasses = new List<SubClass>()
            //                {
            //                    new SubClass() { Name = "Baking", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Fishing", ImgUrl = imgUrl},
            //                    new SubClass() { Name = "Cleaning", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Fishing", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Fishing5", ImgUrl = imgUrl }
            //                }
            //            },
            //        new Factor()
            //            {
            //                Class = "Gamming" ,
            //                SubClasses = new List<SubClass>()
            //                {
            //                    new SubClass() { Name = "PS4", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "XBOX", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "GameBoy", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "Tetris", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "PS3", ImgUrl = imgUrl },
            //                    new SubClass() { Name = "PS2", ImgUrl = imgUrl }
            //                }
            //            }
            //    };

            var imgUrl = "";
            var firstReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.1, lng = 0.1 },
                MatchFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "sport",
                       SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Soccer", ImgUrl = imgUrl },
                                new SubClass() { Name = "Basketball", ImgUrl = imgUrl },
                            }
                   },
                   new Factor()
                   {
                       Class = "gamming",
                       SubClasses = new List<SubClass>() { new SubClass() { Name = "ps4", ImgUrl = imgUrl } }
                   },
                   new Factor()
                   {
                       Class = "work",
                       SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Prog", ImgUrl = imgUrl },
                            }
                    }
                }
            };
            var secReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.5, lng = 0.1 },
                MatchFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "sport",
                       SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Soccer", ImgUrl = imgUrl },
                                new SubClass() { Name = "Basketball", ImgUrl = imgUrl },
                            }
                   },
                   new Factor()
                   {
                       Class = "gamming",
                       SubClasses = new List<SubClass>() { new SubClass() { Name = "ps4", ImgUrl = imgUrl } }
                   },
                   new Factor()
                   {
                       Class = "work",
                       SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Prog", ImgUrl = imgUrl },
                            }
                    }
                }
            };

            var thirdReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.1, lng = 0.1 },
                MatchFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "sport",
                       SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Soccer", ImgUrl = imgUrl },
                                new SubClass() { Name = "Basketball", ImgUrl = imgUrl },
                            }
                   },
                   new Factor()
                   {
                       Class = "gamming",
                       SubClasses = new List<SubClass>() { new SubClass() { Name = "ps4", ImgUrl = imgUrl } }
                   },
                }
            };

            var fourthReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.1, lng = 0.1 },
                MatchFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "gamming",
                       SubClasses = new List<SubClass>() { new SubClass() { Name = "ps4", ImgUrl = imgUrl } }
                   },
                }
            };

            var first = new MatchRequest();
            first.MatchReqDetails = firstReq;
            var sec = new MatchRequest();
            sec.MatchReqDetails = secReq;
            var third = new MatchRequest();
            third.MatchReqDetails = thirdReq;
            var fourth = new MatchRequest();
            fourth.MatchReqDetails = fourthReq;

            manager.CreateMatchRequest(first);
            manager.CreateMatchRequest(third);
            manager.CreateMatchRequest(sec);
            manager.CreateMatchRequest(fourth);
        }


        [HttpGet]
        public async Task Test5()
        {
            var imgUrl = "";
            MatchManager manager = MatchManager.GetManagerInstance();
            MatchReqHandler handler = MatchReqHandler.GetMatchReqHandlerInstance(AlgorithemsTypes.IntuitiveMatchAlg);
            var first = new MatchRequest();
            var firstReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.5, lng = 0.1 },
                MatchFactors = new List<Factor>()
                   {
                       new Factor()
                       {
                           Class = "sport",
                           SubClasses = new List<SubClass>()
                                {
                                    new SubClass() { Name = "Soccer", ImgUrl = imgUrl },
                                    new SubClass() { Name = "Basketball", ImgUrl = imgUrl },
                                }
                       },
                       new Factor()
                       {
                           Class = "gamming",
                           SubClasses = new List<SubClass>() { new SubClass() { Name = "ps4", ImgUrl = imgUrl } }
                       },
                       new Factor()
                       {
                           Class = "work",
                           SubClasses = new List<SubClass>()
                                {
                                    new SubClass() { Name = "Prog", ImgUrl = imgUrl },
                                }
                        }
                    }
            };
            first.MatchReqDetails = firstReq;
            manager.CreateMatchRequest(first);
        }
    }
}
