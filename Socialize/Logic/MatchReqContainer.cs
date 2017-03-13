using Socialize.Exeptions;
using Socialize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Logic
{
    public class MatchReqContainer
    {
        //singlton implementation
        private static MatchReqContainer ReqContainerInstance;

        private Dictionary<int, MatchRequest> MatchRequests;
        private Queue<int> RequestsQ;


        public static MatchReqContainer GetMatchReqContainerInstance()
        {
            if (ReqContainerInstance == null)
            {
                ReqContainerInstance = new MatchReqContainer();
            }
            return ReqContainerInstance;
        }

        private MatchReqContainer()
        {
            MatchRequests = new Dictionary<int, MatchRequest>();
            RequestsQ = new Queue<int>();
        }

        // Add new match request to the dictionary and the q
        public void AddNewMatchReq(MatchRequest matchReq)
        {
            MatchRequests[matchReq.Id] = matchReq;
            RequestsQ.Enqueue(matchReq.Id);
        }

        //Update specific match request by id with new location
        public void UpdateMatchReq(int matchReqId, Location location)
        {
            if (MatchRequests.ContainsKey(matchReqId))
            {
                MatchRequests[matchReqId].MatchReqDetails.Location = location;
            }
            else
            {
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");
            }
        }

        //Remove specific match request from the dictionary only
        public void RemoveMatchReq(int matchReqId)
        {
            if (MatchRequests.ContainsKey(matchReqId))
            {
                MatchRequests.Remove(matchReqId);
            }
            else
            {
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");
            }
        }

        //Suspend match request in case optional match found
        public void SuspendMatchReq(int matchReqId)
        {
            if (MatchRequests.ContainsKey(matchReqId))
            {
                MatchRequests[matchReqId].WaitForOptionalMatchRes = true;
            }
            else
            {
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");
            }
        }

        //Restore match request in case optional match declined
        public void RestoreMatchReq(int matchReqId)
        {
            if (MatchRequests.ContainsKey(matchReqId))
            {
                MatchRequests[matchReqId].WaitForOptionalMatchRes = false;
            }
            else
            {
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");
            }
        }

        //Enqueue the next match request for optional match calculation
        public MatchRequest GetNextMatchRequest()
        {
            if(RequestsQ.Count != 0)
            {
                var nextReqId = RequestsQ.Dequeue();
                return MatchRequests[nextReqId];
            }
            return null;
        }

        //Get all match requests from the dictionary to calculate in the algorithem
        public MatchRequest[] GetAllOtherRequests(int matchReqId)
        {
            if(!MatchRequests.ContainsKey(matchReqId))
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");

            return MatchRequests.Where(x => x.Key != matchReqId).Select(x => x.Value).ToArray();
        }

        //Re-enter match request to the end of the queue
        public void RepositionMatchReq(int matchReqId)
        {
            if (MatchRequests.ContainsKey(matchReqId))
            {
                RequestsQ.Enqueue(matchReqId);
            }
            else
            {
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");
            }
        }
    }
}