using log4net;
using Newtonsoft.Json;
using Socialize.Exeptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public delegate Task EventHandler(object e, OptionalMatchEventArgs args);

        //Delegete function which the matchManager subscribed to
        public event EventHandler OptionalMatchFound;

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //define the minimum value of match strength, below this -> no optional match
        //private double MIN_MATCH_STRENGTH => 50;
        //define the maximum value of match request life time, above this -> match request removed
        private int MAX_MATCHREQ_LIFE_TIME => 60000;
        //singlton implementation
        private static MatchReqHandler ReqHandlerInstance;

        private MatchReqContainer ReqContainerInstance;
        private IMatchAlg MatchAlg;

        //singlton implementation
        public static MatchReqHandler GetMatchReqHandlerInstance(AlgorithemsTypes algType)
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
        private MatchReqHandler(AlgorithemsTypes algType)
        {
            ReqContainerInstance = MatchReqContainer.GetMatchReqContainerInstance();
            MatchAlg = MatchAlgFactory.GetMatchAlg(AlgorithemsTypes.IntuitiveMatchAlg);
        }

        //Event Raised function
        protected virtual void OnOptionalMatchFound(Dictionary<int, int> algResult)
        {
            //Check if there is a listener in the container
            if (OptionalMatchFound != null)
            {
                var parseObj = JsonConvert.SerializeObject(algResult);
                Log.Debug($"Found optional match with results: {parseObj}");
                //Notify matchManager that optional match found
                OptionalMatchFound(this, new OptionalMatchEventArgs() { results = algResult } );
                return;
            }

            else
            {
                Log.Debug("MatchManager doesn't subscribed to events handler");
                throw new MatchRequestHandlerException("MatchMAnager doesn't subscribed to events handler");
            }
        }

        public async Task SendMatchReqToFindMatch()
        {
            var nextMatchReq = ReqContainerInstance.GetNextMatchRequest();
            if (nextMatchReq == null) return;

            //Remove the match request in case the user didn't send updates for more then two minutes
            if(SocializeUtil.IsDateDeprecated(nextMatchReq.Updated, MAX_MATCHREQ_LIFE_TIME))
            {
                await ReqContainerInstance.RemoveMatchReq(nextMatchReq.Id);
                return;
            }

            var allOtherMatchReq = ReqContainerInstance.GetAllOtherRequests(nextMatchReq.Id);

            //Verify the queue holds more than two match requests
            if(nextMatchReq == null || allOtherMatchReq.Length == 0)
            {
                await ReqContainerInstance.RestoreMatchReq(nextMatchReq.Id);
                return;
            }

            

            //Verify match request is not suspended
            if (!nextMatchReq.WaitForOptionalMatchRes)
            {
                foreach (var matchReq in allOtherMatchReq)
                {
                    //Verify other match request not suspended
                    if (!matchReq.WaitForOptionalMatchRes)
                    {
                        var algResult = MatchAlg.CalcOptionalMatch(nextMatchReq, matchReq);

                        //Extract the min match strength value, below this --> no match
                        var minRequestedStrength = algResult.Min(x => x.Value);

                        //If one of the match strength below MIN_MATCH_STRENGTH -> no optional match
                        if (!(algResult.Any(x => x.Value < minRequestedStrength)))
                        {
                            OnOptionalMatchFound(algResult);
                            return;
                        }
                    }
                }
                await ReqContainerInstance.RestoreMatchReq(nextMatchReq.Id);
            }
        }
    }
}