using log4net;
using Newtonsoft.Json;
using Socialize.Models;
using Socialize.Models.GetResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void CreateMatchRequest(MatchRequest matchReq)
        {
            MatchReqContainer.AddNewMatchReq(matchReq);
        }


        //update match request location by id in the container
        public void UpdateMatchRequest(int matchReqId, Location newLocation)
        {
            MatchReqContainer.UpdateMatchReq(matchReqId, newLocation);
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

        //Check if found optional match or if timeout occured
        public void RemoveMatchRequest(int matchReqId)
        {
            MatchReqContainer.RemoveMatchReq(matchReqId);
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

        public UserDataObj GetMatchedUserDetailsByMatchReqId(int matchReqId)
        {
            using(var db = ApplicationDbContext.Create())
            {
                var matchReq = MatchReqContainer.GetMatchReqById(matchReqId);
                var user = db.Users.FirstOrDefault(x => x.Id == matchReq.MatchOwner);

                var factors = user.Factors.Count > 5 ? user.Factors.Take(5) : user.Factors;
                var desc = factors.Select(x => string.Join(",", x.SubClasses.Select(z => z.Name))).ToArray();

                return new UserDataObj()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,

                    ImgUrl = user.ImgUrl,
                    Age = user.Age,

                    Description = desc,
                };
            }
            

        }

        //Invoked Event function, create optional match and add to optional container, and suspends Match requests
        public void OnOptionalMatchFound(object source, OptionalMatchEventArgs args)
        {
            var parseObj = JsonConvert.SerializeObject(args.results);
            Log.Debug($"Creating optional match object with results{parseObj}");

            var firstId = args.results.First().Key;
            var secId = args.results.Last().Key;

            MatchReqContainer.SuspendMatchReq(firstId);
            MatchReqContainer.SuspendMatchReq(secId);

            var firstMatchReq = MatchReqContainer.GetMatchReqById(firstId);
            var secMatchReq = MatchReqContainer.GetMatchReqById(secId);

            var optionalMatch = OptionalMatchBuilder.CreateOptionalMatch(firstMatchReq, secMatchReq, args.results);
            OptionalMatchContainer.AddOptionalMatch(optionalMatch);
        }

        //Check if optional match accepted by all sides or if timeout occured
        public FinalMatch CheckOptionalMatchStatus(int optionalMatchId)
        {
            var optionalMatch = OptionalMatchContainer.GetOptionalMatchByOptionalMatchId(optionalMatchId);
        
            return optionalMatch != null ? BuildFinalMatch(optionalMatch) : null;
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

                //db.FinalMatches.Add(finalMatch);
                //db.SaveChanges();

                return finalMatch;
            }
        }
    }
}