using log4net;
using Socialize.Models;
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

        //Invoked Event function, create optional match and add to optional container, and suspends Match requests
        public void OptionalMatchFound(Dictionary<int, int> algResult)
        {
            var firstId = algResult.First().Key;
            var secId = algResult.Last().Key;

            MatchReqContainer.SuspendMatchReq(firstId);
            MatchReqContainer.SuspendMatchReq(secId);

            var firstMatchReq = MatchReqContainer.GetMatchReqById(firstId);
            var secMatchReq = MatchReqContainer.GetMatchReqById(secId);

            var optionalMatch = OptionalMatchBuilder.CreateOptionalMatch(firstMatchReq, secMatchReq, algResult);
            OptionalMatchContainer.AddOptionalMatch(optionalMatch);
        }

    }
}