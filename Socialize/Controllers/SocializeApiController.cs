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

namespace Socialize.Controllers
{
    public class SocializeApiController : ApiController
    {
        //Update specific match request by id with new location 
        [HttpPost]
        public async Task UpdateMatcReq(MatchReqUpdateObj matchReqUpdate)
        {
            if (FakeDataUtil.Fake)
                return;

            throw new NotImplementedException();

        }

        //Create new match request by id
        [HttpPost]
        public async Task<int> CreateMatcReq(MatchReqDetails newMatchReq)
        {
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

        //Check if found optional match for match request by id
        [HttpGet]
        public async Task<OptinalMatchObj> CheckMatcReqStatus(int matchReqId)
        {
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeOptionalMatch();

            throw new NotImplementedException();
        }

        //Check optional match status - if confirmed by all other participants
        [HttpGet]
        public async Task<FinalMatchObj> CheckOptionalMatchStatus(int optionalMatchId)
        {
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeFinalMatch();

            throw new NotImplementedException();

        }

        //Confirm optional match suggestion
        [HttpPost]
        public async Task AcceptOptionalMatch(EntityIdObj id)
        {
            if (FakeDataUtil.Fake)
                return;

            throw new NotImplementedException();
        }

        //Decline optional match suggestion
        [HttpPost]
        public async Task DeclineOptionalMatch(EntityIdObj id)
        {
            if (FakeDataUtil.Fake)
                return;

            throw new NotImplementedException();
        }

        //Update user detail by user id
        [HttpPost]
        public async Task UpdateUserData(UpdateUserObj updateUserData)
        {
            throw new NotImplementedException();
        }

        //Get user detail by user id
        [HttpGet]
        public async Task<UserDataObj> GetUserData()
        {
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeUserData();

            throw new NotImplementedException();
        }

        //Get all the available factors for user registration
        [HttpGet]
        public async Task<FactorObj[]> GetAllSystemFactors()
        {
            if (FakeDataUtil.Fake)
                return FakeDataUtil.CreateFakeFactors();

            throw new NotImplementedException();
        }

        //[HttpGet]
        //public async Task<MatchReqDetails> Test()
        //{
        //    var details = new MatchReqDetails()
        //    {
        //        location = new Location() { lat = 0.1, lng = 0.2 },
        //        MatchFactors = new List<Factor>() { new Factor() { Class = "dd" } }
        //    };

        //    return details;
        //}

    }
}
