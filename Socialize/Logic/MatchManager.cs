using Socialize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Logic
{
    public class MatchManager
    {
        //singlton implementation
        private static MatchManager ManagerInstance;

        private MatchReqContainer MatchReqContainer;
        private OptionalMatchContainer OptionalMatchContainer;
        private OptionalMatchBuilder OptionalMatchBuilder;

        public static MatchManager GetManagerInstance()
        {
            if(ManagerInstance == null)
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

        public void CreateMatchRequest(MatchRequest matchReq)
        {
            MatchReqContainer.AddNewMatchReq(matchReq);

            int x = 90;
        }

    }
}