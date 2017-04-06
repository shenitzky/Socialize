﻿using log4net;
using Newtonsoft.Json;
using Socialize.Exeptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Logic
{
    /*
     * OptionalMatchEventArgs - send the result of the optional match found as event argument
     */
     public class OptionalMatchEventArgs : EventArgs
    {
        public Dictionary<int, int> results { get; set; }
    }

    /*
     * MatchReqHandler - responsible for sending match requests to match algorithm and fire event
     * to match manager in case optional match found
     */
    public class MatchReqHandler
    {
        //Delegete function which the matchManager subscribed to
        //public delegate void OptionalMatchFoundEventHandler(object source, OptionalMatchEventArgs args);
        //public event OptionalMatchFoundEventHandler OptionalMatchFound;
        public event EventHandler<OptionalMatchEventArgs> OptionalMatchFound;

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //define the minimum value of match strength, below this -> no optional match
        private double MIN_MATCH_STRENGTH = 50;
         
        //singlton implementation
        private static MatchReqHandler ReqHandlerInstance;

        private MatchReqContainer ReqContainerInstance;
        private IMatchAlg MatchAlg;

        //singlton implementation
        public static MatchReqHandler GetMatchReqHandlerInstance(MatchAlgFactory.AlgorithemsTypes algType)
        {
            if (ReqHandlerInstance == null)
            {
                ReqHandlerInstance = new MatchReqHandler(algType);

                //Subscribed the MatchManager to the event handler
                ReqHandlerInstance.OptionalMatchFound += MatchManager.GetManagerInstance().OnOptionalMatchFound;
            }
            return ReqHandlerInstance;
        }

        //singlton implementation
        private MatchReqHandler(MatchAlgFactory.AlgorithemsTypes algType)
        {
            ReqContainerInstance = MatchReqContainer.GetMatchReqContainerInstance();
            MatchAlg = MatchAlgFactory.GetMatchAlg(MatchAlgFactory.AlgorithemsTypes.IntuitiveMatchAlg);
            //Set maximum distance between two locations to 25
            MatchAlg.MAX_DISTANCE = 25;
        }

        //Event Raised function
        protected virtual void OnOptionalMatchFound(Dictionary<int, int> res)
        {
            if (OptionalMatchFound != null)
            {
                var parseObj = JsonConvert.SerializeObject(res);
                Log.Debug($"Found optional match with results: {parseObj}");
                //Notify matchManager that optional match found
                OptionalMatchFound(this, new OptionalMatchEventArgs() { results = res } );
                return;
            }

            else
            {
                Log.Debug("MatchManager doesn't subscribed to events handler");
                throw new MatchRequestHandlerException(
                "MatchMAnager doesn't subscribed to events handler"
                );
            }
        }

        public void SendMatchReqToFindMatch()
        {
            var nextMatchReq = ReqContainerInstance.GetNextMatchRequest();
            var allOtherMatchReq = ReqContainerInstance.GetAllOtherRequests(nextMatchReq.Id);

            //Verify the queue holds more than two match requests
            if(nextMatchReq == null || allOtherMatchReq == null)
            {
                //TODO - think on optimization for empty / less than two object
                return;
            }

            //Verify next match request not suspended
            if (!nextMatchReq.WaitForOptionalMatchRes)
            {
                //TODO - BETA implement this loop in more efficient way
                foreach (var matchReq in allOtherMatchReq)
                {
                    //Verify other match request not suspended
                    if (!matchReq.WaitForOptionalMatchRes)
                    {
                        var res = MatchAlg.CalcOptionalMatch(nextMatchReq, matchReq);

                        //Check if there is optional match properties
                        if(res != null)
                        {
                            //If one of the match strength below MIN_MATCH_STRENGTH -> no optional match
                            if (!(res.Any(x => x.Value < MIN_MATCH_STRENGTH)))
                            {
                                OnOptionalMatchFound(res);
                                return;
                            }
                        }
                    }
                }
            }

        }
    }
}