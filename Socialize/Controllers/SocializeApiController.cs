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

namespace Socialize.Controllers
{
    
    public class SocializeApiController : ApiController
    {
        //Update specific match request by id with new location 
        [HttpPost]
        public async Task UpdateMatcReq(MatchReqUpdateObj matchReqUpdate)
        {

        }

        //Create new match request by id
        [HttpPost]
        public async Task CreateMatcReq(MatchRequestObj newMatchReq)
        {
            using (var db = ApplicationDbContext.Create())
            {
                var userId = User.Identity.GetUserId();
                var users = db.Users.Where(user => user.Id == userId).ToArray();
                int x = 90;
            }
        }

        //Check if found optional match for match request by id
        [HttpGet]
        public async Task<IOptionalMatch> CheckMatcReqStatus(int matchReqId)
        {
            var optionalMatch = new OneOnOneOptionalMatch()
            {
                Id = 001,
                Created = DateTime.Now,
                MatchedUsersDetails = new MatchReqDetails[] { new MatchReqDetails(), new MatchReqDetails() },
                MatchRequestIds = new List<int> { 1, 2 },
                MatchStrength = new List<int> { 80, 90 },
                Status = new Dictionary<int, bool> { { 1, true }, { 2, true } }
            };

            return optionalMatch;
        }

        //Check optional match status - if confirmed by all other participants
        [HttpGet]
        public async Task<ApplicationUser> CheckOptionalMatchStatus(int optionalMatchId)
        {
            return null;
            
        }

        //Confirm optional match suggestion
        [HttpPost]
        public async Task AcceptOptionalMatch(int optionalMatchId)
        {

        }

        //Decline optional match suggestion
        [HttpPost]
        public async Task DeclineOptionalMatch(int optionalMatchId)
        {

        }

        //Update user detail by user id
        [HttpPost]
        public async Task UpdateUserData(UpdateUserObj updateUserData)
        {

        }

        //Get user detail by user id
        [HttpGet]
        public async Task<UserDataObj> GetUserData(string userId)
        {

            return null;
        }

        //Get all the available factors for user registration
        [HttpGet]
        public async Task<Factor[]> GetAllSystemFactors()
        {

            return null;
        }
    }
}
