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

        //Check if found optional match for match request by id (loop)
        //[HttpGet]
        //public async Task<OptinalMatchObj> CheckMatcReqStatus(int matchReqId)
        //{
        //    Log.Debug($"GET CheckMatcReqStatus calld with id {matchReqId}");
        //    if (FakeDataUtil.Fake)
        //        return FakeDataUtil.CreateFakeOptionalMatch();

        //    var matchManager = MatchManager.GetManagerInstance();
        //    var optionalMatch = matchManager.CheckMatchRequestStatus(matchReqId);
        //    return optionalMatch != null ? SocializeUtil.ConvertToOptinalMatchObj(optionalMatch, matchReqId) : null;
        //}

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
        public async Task<FinalMatchObj> CheckOptionalMatchStatus(int optionalMatchId)
        {
            Log.Debug($"GET CheckOptionalMatchStatus calld with id {optionalMatchId}");
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeFinalMatch();

            throw new NotImplementedException();
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
                var user = db.Users.FirstOrDefault(x => x.Id == userId);

                if (user == null)
                    throw new Exception($"Can not find userId- {userId}");

                var newFactors = updateUserData.Data.Select(x => new Factor() { Class = x.Class, SubClasses = x.SubClasses, UserId = userId }).ToList();
                var originalFactors = user.Factors ?? new List<Factor>();

                var unionFactors = originalFactors.Union(newFactors);
                var distinctFactors = unionFactors.DistinctBy(x => x.Class + x.SubClasses).ToList();

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
        public async Task<FactorObj[]> GetAllSystemFactors()
        {
            Log.Debug($"GET GetAllSystemFactors calld");
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeFactors();

            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<List<int>> Test()
        {
            var firstReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.1, lng = 0.1 },
                MatchFactors = new List<Factor>()
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
               }
            };
            var secReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.1, lng = 0.1 },
                MatchFactors = new List<Factor>()
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
               }
            };

            var firstId = await CreateMatcReq(firstReq);
            var secId = await CreateMatcReq(secReq);

            return new List<int>() { firstId, secId };
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
        public async Task Test3()
        {
            MatchManager manager = MatchManager.GetManagerInstance();
            MatchReqHandler handler = MatchReqHandler.GetMatchReqHandlerInstance(MatchAlgFactory.AlgorithemsTypes.IntuitiveMatchAlg);
            manager.CreateMatchRequest(new MatchRequest());
            manager.CreateMatchRequest(new MatchRequest());
            handler.SendMatchReqToFindMatch();
        }

        [HttpGet]
        public async Task Test4()
        {
            MatchManager manager = MatchManager.GetManagerInstance();
            MatchReqHandler handler = MatchReqHandler.GetMatchReqHandlerInstance(MatchAlgFactory.AlgorithemsTypes.IntuitiveMatchAlg);


            var firstReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.1, lng = 0.1 },
                MatchFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "sport",
                       SubClasses = new List<string>() { "soccer", "basketball" }
                   },
                   new Factor()
                   {
                       Class = "gamming",
                       SubClasses = new List<string>() { "ps4" }
                   },
                   new Factor()
                   {
                       Class = "work",
                       SubClasses = new List<string>() { "eng" }
                   }
               }
            };
            var secReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.1, lng = 0.1 },
                MatchFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "gamming",
                       SubClasses = new List<string>() { "ps4","xbox","gameboy" }
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
                       SubClasses = new List<string>() { "tennis" }
                   },
                   new Factor()
                   {
                       Class = "gamming",
                       SubClasses = new List<string>() { "xbox" }
                   }
               }
            };

            var fourthReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.1, lng = 0.1 },
                MatchFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "work",
                       SubClasses = new List<string>() { "xbox" }
                   }
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


            handler.SendMatchReqToFindMatch();
        }

    }
}
