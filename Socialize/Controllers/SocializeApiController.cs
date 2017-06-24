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
            //newMatchReq.minMatchStrength = newMatchReq.maxDistance;
            if (newMatchReq.MatchFactors == null || newMatchReq.MatchFactors.Count == 0 || newMatchReq.minMatchStrength <= 0 || newMatchReq.minMatchStrength > 100)
                return -1;

            var matchManager = MatchManager.GetManagerInstance();
            var userId = User.Identity.GetUserId();

            var userPendingMatchReqId = matchManager.GetMatchReqIdByUser(userId);
            if (userPendingMatchReqId != -1)
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

            if (matchReqId == -1)
            {
                return new OptinalMatchObj
                {
                    Id = -1
                };
            }
            //If optional match found
            if (optionalMatch != null)
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
            await manager.AcceptOrDeclineOptionalMatch(ids.OptionalMatchId, ids.MatchReqId, true);
        }

        //Decline optional match suggestion
        [HttpPost]
        public async Task DeclineOptionalMatch(OptionalMatchIdAndMatchReqIdObj ids)
        {
            var parseObj = JsonConvert.SerializeObject(ids);
            Log.Debug($"POST DeclineOptionalMatch calld with id object {parseObj}");

            var manager = MatchManager.GetManagerInstance();
            await manager.AcceptOrDeclineOptionalMatch(ids.OptionalMatchId, ids.MatchReqId, false);
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

                db.Factors.RemoveRange(originalFactors);

                var factorToInsert = new List<Factor>();
                foreach (var rawFactor in updateUserData.Data)
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

        //Get user pending optional match
        [HttpGet]
        public async Task<OptinalMatchObj> GetUserOptionalMatch()
        {
            var userId = User.Identity.GetUserId();
            var manager = MatchManager.GetManagerInstance();

            var optionalMatch = manager.GetOptionalMatchByOwnerId(userId);
            if (optionalMatch == null)
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

        //Set user informations
        [HttpPost]
        public async Task UpdateUserExtraData(UserExtraData data)
        {
            using (var db = ApplicationDbContext.Create())
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
            using (var db = ApplicationDbContext.Create())
            {
                var factors = db.Factors.Include(x => x.SubClasses).Where(x => x.UserId == null).ToArray();
                return factors;
            }
        }

        //Get pair of images for effect on the client gui
        [HttpGet]
        public async Task<ImagesObj[]> GetImagesForBubble()
        {
            using (var db = ApplicationDbContext.Create())
            {
                var allImages = db.AvatarImgs.ToArray();
                var images = new List<ImagesObj>();

                var length = allImages.Length % 2 == 0 ? allImages.Length : allImages.Length - 1;
                for (var i = 0; i < length; i++)
                {
                    images.Add(new ImagesObj(allImages[i].ImgUrl, allImages[i + 1].ImgUrl));
                }

                return images.ToArray();
            }
        }

        //Add all possible factors to the DB
        [HttpGet]
        public async Task AddFactorToDb()
        {
            SocializeUtil.CreateFactors();
        }

        //Log out 
        [HttpGet]
        public void Logoff()
        {
            var AutheticationManager = HttpContext.Current.GetOwinContext().Authentication;
            AutheticationManager.SignOut();
        }

        //Add user images to DB
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

        //Return the data struct content for analysis
        [HttpGet]
        public async Task<DataStructStatusObj> GetDataStructStatus()
        {
            var matchReqContainer = MatchReqContainer.GetMatchReqContainerInstance();
            var optionalMatchContainer = OptionalMatchContainer.GetOptionalMatchContainerInstance();

            return new DataStructStatusObj()
            {
                OptionalMatches = optionalMatchContainer.OptionalMatches,
                MatchRequests = matchReqContainer.MatchRequests,
                Q = matchReqContainer.RequestsQ.ToArray()
            };
        }

        //Suggest new sub-class to the system by the user
        [HttpGet]
        public async Task SuggestNewSubClass(string newSubClassDesc)
        {
            var factorsManager = NewFactorsManager.GetInstance();
            factorsManager.AddNewSuggestedFactor(newSubClassDesc);
        }
    }
}
