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
using Microsoft.AspNet.Identity.Owin;

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
            


            //
            var matchManager = MatchManager.GetManagerInstance();
            var userId = User.Identity.GetUserId();

            var userPendingMatchReqId = matchManager.GetMatchReqIdByUser(userId);
            if(userPendingMatchReqId != -1)
            {
                var errParseObj = JsonConvert.SerializeObject(newMatchReq);
                Log.Debug($"POST cannot create match request for user {userId}, there is pending match request on the system ");
                return userPendingMatchReqId;
            }

            var parseObj = JsonConvert.SerializeObject(newMatchReq);
            Log.Debug($"POST CreateMatcReq calld with match details {parseObj}");
            using (var db = ApplicationDbContext.Create())
            {
                var matchReq = new MatchRequest();
                matchReq.MatchReqDetails = newMatchReq;
                matchReq.MatchOwner = userId;

                //save match request log
                var logMatchReq = new MatchRequestLog()
                {
                    Created = DateTime.Now,
                    Location = JsonConvert.SerializeObject(newMatchReq.Location),
                    MatchFactors = JsonConvert.SerializeObject(newMatchReq.MatchFactors),
                    maxDistance = newMatchReq.maxDistance,
                    UserId = userId
                };

                db.MatchRequestLog.Add(logMatchReq);
                db.SaveChanges();

                
                await matchManager.CreateMatchRequest(matchReq);

                return matchReq.Id;
            }
        }

        //Check if found optional match for match request by id (loop) if not Update specific match request by id with new location 
        [HttpPost]
        public async Task<OptinalMatchObj> UpdateAndCheckMatcReq(MatchReqUpdateObj matchReqUpdate)
        {
            var parseObj = JsonConvert.SerializeObject(matchReqUpdate);
            Log.Debug($"POST UpdateAndCheckMatcReq calld with updates {parseObj}");

            var matchManager = MatchManager.GetManagerInstance();
            var optionalMatch = matchManager.CheckMatchRequestStatus(matchReqUpdate.matchReqId);

            //Check if match request exist (depricated)
            var userId = User.Identity.GetUserId();
            var matchReqId = matchManager.GetMatchReqIdByUser(userId);

            if(matchReqId == -1)
            {
                return new OptinalMatchObj
                {
                    Id = -1
                };
            }

            
            //If optional match found
            if(optionalMatch != null)
            {
                using (var db = ApplicationDbContext.Create())
                {
                    //save optional match log
                    var logOptionalMatch = new OptionalMatchLog()
                    {
                        UserId = User.Identity.GetUserId(),
                        Created = optionalMatch.Created,
                        MatchedFactors = JsonConvert.SerializeObject(optionalMatch.MatchedFactors),
                        MatchStrength = JsonConvert.SerializeObject(optionalMatch.MatchStrength)
                    };


                    db.OptionalMatchLog.Add(logOptionalMatch);
                    await db.SaveChangesAsync();
                }
                
                //Get matched user details
                var matchedMatchReqId = optionalMatch.MatchRequestIds.Where(x => x != matchReqUpdate.matchReqId).FirstOrDefault();
                var matchedUserDetails = matchManager.GetMatchedUserDetailsByMatchReqId(matchedMatchReqId);

                var result = SocializeUtil.ConvertToOptinalMatchObj(optionalMatch, matchReqUpdate.matchReqId, matchedUserDetails);
                return result;
            }

            Log.Debug($"Optional match not found for match req: {matchReqUpdate.matchReqId}");

            var manager = MatchManager.GetManagerInstance();
            await manager.UpdateMatchRequest(matchReqUpdate.matchReqId, matchReqUpdate.location);

            Log.Debug($"Match request id: {matchReqUpdate.matchReqId} location updated");
            return null;
        }

        //Confirm optional match suggestion
        [HttpPost]
        public async Task AcceptOptionalMatch(OptionalMatchIdAndMatchReqIdObj ids)
        {
            var parseObj = JsonConvert.SerializeObject(ids);
            Log.Debug($"POST AcceptOptionalMatch calld with OptionalMatchIdAndMatchReqIdObj object {parseObj}");

            var manager = MatchManager.GetManagerInstance();
            manager.AcceptOrDeclineOptionalMatch(ids.OptionalMatchId, ids.MatchReqId, true);
        }

        //Decline optional match suggestion
        [HttpPost]
        public async Task DeclineOptionalMatch(OptionalMatchIdAndMatchReqIdObj ids)
        {
            var parseObj = JsonConvert.SerializeObject(ids);
            Log.Debug($"POST DeclineOptionalMatch calld with id object {parseObj}");

            var manager = MatchManager.GetManagerInstance();
            manager.AcceptOrDeclineOptionalMatch(ids.OptionalMatchId, ids.MatchReqId, false);
        }

        //Check optional match status - if confirmed by all other participants (loop)
        [HttpGet]
        public async Task<FinalMatchObj> CheckOptionalMatchStatus(int optionalMatchId, int matchReqId)
        {
            Log.Debug($"GET CheckOptionalMatchStatus calld with id {optionalMatchId}");

            var manager = MatchManager.GetManagerInstance();

            //Remove the optional match in case it alive more then 20 sec
            if (manager.IsOptionalMatchDeprecate(optionalMatchId))
            {
                await manager.RemoveMatchRequestsByOptionalMatchId(optionalMatchId);
                manager.RemoveOptionalMatchById(optionalMatchId);
                return new FinalMatchObj()
                {
                    IsAccepted = false,
                };
            }

            var finalMatch = manager.CheckOptionalMatchStatus(optionalMatchId);

            if (finalMatch != null)
            {
                if (finalMatch.IsAccepted)
                {
                    manager.SetFinalMatchReceivedForOptionalMatch(optionalMatchId, matchReqId);
                    //Remove the match requests and optional match in case both sides receive a final match
                    if (manager.CheckIfFinalMatchReceived(optionalMatchId))
                    {
                        await manager.RemoveMatchRequestsByOptionalMatchId(optionalMatchId);
                        manager.RemoveOptionalMatchById(optionalMatchId);
                    }
                }

                var userId = User.Identity.GetUserId();
                return SocializeUtil.ConvertToFinalMatchObj(finalMatch, matchReqId, userId);

            }
            return null;
        }

        //Update user detail by user id
        [HttpPost]
        public async Task UpdateUserData(UpdateUserObj updateUserData)
        {
            var parseObj = JsonConvert.SerializeObject(updateUserData);
            Log.Debug($"POST UpdateUserData calld with user object {parseObj}");

            using (var db = ApplicationDbContext.Create())
            {
                
                var userId = User.Identity.GetUserId();
                var user = db.Users.Include(x => x.Factors).Include(x => x.Factors.Select(z => z.SubClasses)).FirstOrDefault(x => x.Id == userId);

                var originalFactors = user.Factors;
                var rawOriginalSubClasses = originalFactors.Select(x => x.SubClasses.ToArray());
                
                //TODO: delete subClasses from db

                db.Factors.RemoveRange(originalFactors);

                var factorToInsert = new List<Factor>();
                foreach(var rawFactor in updateUserData.Data)
                {
                    var factor = new Factor()
                    {
                        Class = rawFactor.Class,
                        SubClasses = rawFactor.SubClasses,
                        UserId = userId
                    };

                    db.Factors.Add(factor);

                    factorToInsert.Add(factor);
                }
                await db.SaveChangesAsync();
            }
        }

        //Get user detail by user id
        [HttpGet]
        public async Task<UserDataObj> GetUserData()
        {
            try
            {
                Log.Debug($"GET GetUserData calld");
                using (var db = ApplicationDbContext.Create())
                {
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Include(x => x.Factors).Include(x => x.Factors.Select(f => f.SubClasses)).FirstOrDefault(x => x.Id == userId);

                    if (user == null)
                        throw new Exception("user not found");

                    return new UserDataObj()
                    {
                        Age = user.Age,
                        FirstName = user.FirstName ?? "Moshe",
                        LastName = user.LastName ?? "Levi",
                        Id = user.Id,
                        ImgUrl = user.ImgUrl ?? db.AvatarImgs.First().ImgUrl,
                        Mail = user.Email,
                        Premium = false,

                        Factors = user.Factors?.ToArray()
                    };
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        //Get user pending optional match
        [HttpGet]
        public async Task<OptinalMatchObj> GetUserOptionalMatch()
        {
            var userId = User.Identity.GetUserId();
            var manager = MatchManager.GetManagerInstance();

            var optionalMatch = manager.GetOptionalMatchByOwnerId(userId);
            if(optionalMatch == null)
            {
                return null;
            }
            var userPendingMatchReqId = manager.GetMatchReqIdByUser(userId);

            var matchedMatchReqId = optionalMatch.MatchRequestIds.Where(x => x != userPendingMatchReqId).FirstOrDefault();

            var matchedUserDetails = manager.GetMatchedUserDetailsByMatchReqId(matchedMatchReqId);

            return SocializeUtil.ConvertToOptinalMatchObj(optionalMatch, userPendingMatchReqId, matchedUserDetails);
        }

        //Get conected user img url
        [HttpGet]
        public async Task<string> GetUserImgUrl()
        {
            var userId = User.Identity.GetUserId();
            return SocializeUtil.GetUserImg(userId);
        }

        [HttpPost]
        public async Task UpdateUserExtraData(UserExtraData data)
        {
            using(var db = ApplicationDbContext.Create())
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.FirstOrDefault(x => x.Id == userId);

                user.FirstName = data.FirstName;
                user.LastName = data.LastName;
                user.Description = data.Description;
                user.PhoneNumber = data.PhoneNumber;
                user.Age = data.Age;

                await db.SaveChangesAsync();
            }
        }

        //Get all the available factors for user registration
        [HttpGet]
        public async Task<Factor[]> GetAllSystemFactors()
        {
            Log.Debug($"GET GetAllSystemFactors calld");
            using(var db = ApplicationDbContext.Create())
            {
                var factors = db.Factors.Include(x => x.SubClasses).Where(x => x.UserId == null).ToArray();
                return factors;
            }
        }

        [HttpGet]
        public async Task AddFactorToDb()
        {
            FakeDataUtil.CreateFactors();
        }

        //Log out 
        [HttpGet]
        public void Logoff()
        {
            var AutheticationManager = HttpContext.Current.GetOwinContext().Authentication;
            AutheticationManager.SignOut();
        }

        //[HttpGet]
        //public async Task<object> GetAddressFromCoords(string lat1, string lang1, string lat2, string lang2)
        //{
        //    var firstAdress = SocializeUtil.RetrieveFormatedAddress(lat1, lang1);
        //    var secAdress = SocializeUtil.RetrieveFormatedAddress(lat2, lang2);

        //    if (firstAdress == null || secAdress == null)
        //        return null;

        //    return new
        //    {
        //        FirstAddress = firstAdress,
        //        SecAddress = secAdress
        //    };
        //}

        [HttpGet]
        public async Task AddAvatarImg()
        {
            using (var db = ApplicationDbContext.Create())
            {
                var all = db.AvatarImgs.Select(x => x);

                db.AvatarImgs.RemoveRange(all);

                var domain = HttpContext.Current.Request.Url.Authority;
                var imgUrl = $"http://{domain}/Content/Images/Profiles/profile";

                for (var i = 1; i <= 7; i++)
                {
                    var imageToiInsert = new AvatarImg() { ImgUrl = imgUrl + i + ".png" };
                    db.AvatarImgs.Add(imageToiInsert);
                }

                await db.SaveChangesAsync();

            }
        }

        

        [HttpGet]
        public async Task SignInDefaultUser()
        {
            using(var db = ApplicationDbContext.Create())
            {
                var userMail = "nodler@gmail.com";
                var user = db.Users.FirstOrDefault(x => x.Email == userMail);

                var AutheticationManager = HttpContext.Current.GetOwinContext().Authentication;

                var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var claimsIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                AutheticationManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties { IsPersistent = true }, claimsIdentity);
            }
        }


        [HttpGet]
        public async Task<object> Test(int index)
        {
            switch (index)
            {
                case 0:
                    return FakeDataUtil.CreateFakeFactors(false);
                    break;
                case 1:
                    return FakeDataUtil.CreateFakeFactorsWithoutUrl();
                    break;
                case 2:
                    return FakeDataUtil.CreateFakeFinalMatch();
                    break;
                case 3:
                    return FakeDataUtil.CreateFakeOptionalMatch();
                    break;
                case 4:
                    return FakeDataUtil.CreateFakeUserData();
                    break;
                case 5:
                    return FakeDataUtil.FakeUserImgUrl();
                    break;
            }
            return null;
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
            first.MatchOwner = "71b9ca3f-a85e-40db-a7f3-c4c3373a46b5";
            var sec = new MatchRequest();
            sec.MatchReqDetails = secReq;
            sec.MatchOwner = "74d64e5e-8ae2-4159-a492-a5b0bc0426a2";
            var third = new MatchRequest();
            third.MatchReqDetails = thirdReq;
            third.MatchOwner = "74d64e5e-8ae2-4159-a492-a5b0bc0426a2";
            var fourth = new MatchRequest();
            fourth.MatchReqDetails = fourthReq;
            fourth.MatchOwner = "74d64e5e-8ae2-4159-a492-a5b0bc0426a2";

            await manager.CreateMatchRequest(first);
            await manager.CreateMatchRequest(third);
            await manager.CreateMatchRequest(sec);
            await manager.CreateMatchRequest(fourth);
        }
    }
}
