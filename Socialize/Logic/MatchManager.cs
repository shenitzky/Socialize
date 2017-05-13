using log4net;
using Newtonsoft.Json;
using Socialize.Models;
using Socialize.Models.GetResponseObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Socialize.Logic
{
    public class MatchManager
    {

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //singlton implementation
        private static MatchManager ManagerInstance;

        private MatchReqContainer MatchReqContainer;
        private OptionalMatchContainer OptionalMatchContainer;
        private OptionalMatchBuilder OptionalMatchBuilder;

        //define the maximum value of optional match life time, above this -> optional match removed
        private int MAX_OPTINAL_MATCH_LIFE_TIME => 120000;

        public static MatchManager GetManagerInstance()
        {
            if (ManagerInstance == null)
            {
                ManagerInstance = new MatchManager();
            }
            return ManagerInstance;
        }

        private MatchManager()
        {
            MatchReqContainer = MatchReqContainer.GetMatchReqContainerInstance();
            OptionalMatchContainer = OptionalMatchContainer.GetOptionalMatchContainerInstance();
            OptionalMatchBuilder = new OptionalMatchBuilder();
        }

        //Add new match request to the match request container
        public async Task CreateMatchRequest(MatchRequest matchReq)
        {
            await MatchReqContainer.AddNewMatchReq(matchReq);
        }


        //update match request location by id in the container
        public async Task UpdateMatchRequest(int matchReqId, Location newLocation)
        {
            await MatchReqContainer.UpdateMatchReq(matchReqId, newLocation);
        }

        //Check if found optional match or if timeout occured
        public IOptionalMatch CheckMatchRequestStatus(int matchReqId)
        {
            var matchReqObj = MatchReqContainer.GetMatchReqById(matchReqId);
            if (matchReqObj.WaitForOptionalMatchRes)
            {
                return OptionalMatchContainer.GetOptionalMatchByMatchRequestId(matchReqId);
            }
            return null;
        }
        //Remove all match requests of specific optional match
        public async Task RemoveMatchRequestsByOptionalMatchId(int optionalMatchId)
        {
            var optionalMatch = OptionalMatchContainer.GetOptionalMatchByOptionalMatchId(optionalMatchId);
            await RemoveMatchRequest(optionalMatch.MatchRequestIds.First());
            await RemoveMatchRequest(optionalMatch.MatchRequestIds.Last());
        }

        //Check if found optional match or if timeout occured
        public async Task RemoveMatchRequest(int matchReqId)
        {
            await MatchReqContainer.RemoveMatchReq(matchReqId);
        }

        //Accept (status = true ) or decline (status = false) optional match offer
        public void AcceptOrDeclineOptionalMatch(int optionalMatchId, int matchReqId, bool status)
        {
            var optionalMatch = OptionalMatchContainer.GetOptionalMatchByOptionalMatchId(optionalMatchId);

            //Check if optional match exists (can be removed if first user declined it)
            if(optionalMatch != null)
            {
                //Check the status, if true, update the status else remove the optional match
                if (status)
                {
                    optionalMatch.Status[matchReqId] = true;
                }
                else
                {
                    OptionalMatchContainer.RemoveOptionalMatchByOptionalMatchId(optionalMatchId);
                }
            }
        }

        //Get Matched User Details By Match Req Id for optional match
        public UserDataObj GetMatchedUserDetailsByMatchReqId(int matchReqId)
        {
            using(var db = ApplicationDbContext.Create())
            {
                var matchReq = MatchReqContainer.GetMatchReqById(matchReqId);
                var user = db.Users.Include(x => x.Factors).Include(x => x.Factors.Select(z => z.SubClasses)).FirstOrDefault(x => x.Id == matchReq.MatchOwner);
                if(user != null)
                {
                    var desc = new string[] { };
                    if (user.Factors != null && user.Factors.Count > 0)
                    {
                        var factors = user.Factors.Count > 5 ? user.Factors.Take(5) : user.Factors;
                        desc = factors.Select(x => string.Join(",", x.SubClasses.Select(z => z.Name))).ToArray();
                    }

                    var splitedMail = user.Email.Split('@');
                    return new UserDataObj()
                    {
                        FirstName = user.FirstName ?? splitedMail[0],
                        LastName = user.LastName ?? splitedMail[1],

                        ImgUrl = user.ImgUrl,
                        Age = user.Age,

                        Description = desc,
                    };
                }
                return null;
            }
            

        }
        //Update final match received for match id
        public void SetFinalMatchReceivedForOptionalMatch(int optionalMatchId, int matchReqId)
        {
            OptionalMatchContainer.SetFinalMatchReceivedForOptionalMatch(optionalMatchId, matchReqId);
        }

        //Check if final match received for each user
        public bool CheckIfFinalMatchReceived(int optionalMatchId)
        {
            return OptionalMatchContainer.CheckIfFinalMatchReceived(optionalMatchId);
        }

        //Invoked Event function, create optional match and add to optional container, and suspends Match requests
        public async Task OnOptionalMatchFound(object source, OptionalMatchEventArgs args)
        {
            var parseObj = JsonConvert.SerializeObject(args.results);
            Log.Debug($"Creating optional match object with results{parseObj}");

            var firstId = args.results.First().Key;
            var secId = args.results.Last().Key;

            await MatchReqContainer.SuspendMatchReq(firstId);
            await MatchReqContainer.SuspendMatchReq(secId);

            var firstMatchReq = MatchReqContainer.GetMatchReqById(firstId);
            var secMatchReq = MatchReqContainer.GetMatchReqById(secId);

            var optionalMatch = OptionalMatchBuilder.CreateOptionalMatch(firstMatchReq, secMatchReq, args.results);
            OptionalMatchContainer.AddOptionalMatch(optionalMatch);
        }

        //Check if optional match accepted by all sides or if timeout occured
        public FinalMatch CheckOptionalMatchStatus(int optionalMatchId)
        {
            var optionalMatch = OptionalMatchContainer.GetOptionalMatchByOptionalMatchId(optionalMatchId);

            //Check if one of the sides decline the optional match
            if(optionalMatch == null)
            {
                return BuildDeclinedFinalMatch();
            }
            //Check if still waiting for one of the sides to respond
            else if(!optionalMatch.Status.First().Value || !optionalMatch.Status.Last().Value)
            {
                return null;
            }
            //optional match accepted by all sides  
            return BuildFinalMatch(optionalMatch);
        }

        //Get optional match by owner id
        public IOptionalMatch GetOptionalMatchByOwnerId(string userId)
        {
            var matchReqId = MatchReqContainer.GetMatchReqIdByOwner(userId);
            return matchReqId != -1 ? OptionalMatchContainer.GetOptionalMatchByMatchRequestId(matchReqId) : null;
        }
        //Check if optional match alive more then 20 sec
        public bool IsOptionalMatchDeprecate(int optionalMatchId)
        {
            var optionalMatch = OptionalMatchContainer.GetOptionalMatchByOptionalMatchId(optionalMatchId);
            return SocializeUtil.IsDateDeprecated(optionalMatch.Created, MAX_OPTINAL_MATCH_LIFE_TIME);
        }

        //Remove optional match by id
        public void RemoveOptionalMatchById(int optionalMatchId)
        {
            OptionalMatchContainer.RemoveOptionalMatchByOptionalMatchId(optionalMatchId);
        }

        private FinalMatch BuildDeclinedFinalMatch()
        {
            return new FinalMatch()
            {
                IsAccepted = false
            };
        }

        private FinalMatch BuildFinalMatch(IOptionalMatch optionalMatch)
        {
            using (var db = ApplicationDbContext.Create())
            {
                var firstMatch = MatchReqContainer.GetMatchReqById(optionalMatch.MatchRequestIds.First());
                var secMatch = MatchReqContainer.GetMatchReqById(optionalMatch.MatchRequestIds.Last());

                var matchLocations = new List<Location>() { firstMatch.MatchReqDetails.Location, secMatch.MatchReqDetails.Location };
                var locationAsString = SocializeUtil.ConvertLocationsToString(matchLocations);
                var userIds = new List<string>() { firstMatch.MatchOwner, secMatch.MatchOwner };

                var finalMatch = new FinalMatch()
                {
                    Created = DateTime.Now,
                    Factors = optionalMatch.MatchedFactors,
                    IsAccepted = true,
                    Locations = locationAsString,
                    MatchStrength = optionalMatch.MatchStrength,
                    UsersId = userIds
                };

                //save finale match log
                var finalMatchLog = new FinalMatchLog()
                {
                    Created = finalMatch.Created,
                    Factors = JsonConvert.SerializeObject(finalMatch.Factors),
                    IsAccepted = finalMatch.IsAccepted,
                    Locations = finalMatch.Locations,
                    MatchStrength = JsonConvert.SerializeObject(finalMatch.MatchStrength),
                    UsersId = JsonConvert.SerializeObject(finalMatch.UsersId)
                };

                db.FinalMatchesLog.Add(finalMatchLog);
                db.SaveChanges();

                return finalMatch;
            }

        }
    }
}