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
        [HttpGet]
        public async Task<OptinalMatchObj> CheckMatcReqStatus(int matchReqId)
        {
            Log.Debug($"GET CheckMatcReqStatus calld with id {matchReqId}");
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeOptionalMatch();

            var matchManager = MatchManager.GetManagerInstance();
            var optionalMatch = matchManager.CheckMatchRequestStatus(matchReqId);
            return optionalMatch != null ? SocializeUtil.ConvertToOptinalMatchObj(optionalMatch, matchReqId) : null;
        }

        //Update specific match request by id with new location 
        [HttpPost]
        public async Task UpdateMatcReq(MatchReqUpdateObj matchReqUpdate)
        {
            var parseObj = JsonConvert.SerializeObject(matchReqUpdate);
            Log.Debug($"POST UpdateMatcReq calld with updates {parseObj}");
            if (FakeDataUtil.Fake)
                return;
            var manager = MatchManager.GetManagerInstance();
            manager.UpdateMatchRequest(matchReqUpdate.matchReqId, matchReqUpdate.location);
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
            throw new NotImplementedException();
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
            manager.OptionalMatchFound(result);

        }


    }
}
