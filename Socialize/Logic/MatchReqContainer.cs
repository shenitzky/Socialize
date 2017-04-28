using log4net;
using Newtonsoft.Json;
using Socialize.Exeptions;
using Socialize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Socialize.Logic
{
    public class MatchReqContainer
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        public async Task AddNewMatchReq(MatchRequest matchReq)
        {
            var parseObj = JsonConvert.SerializeObject(matchReq);
            Log.Debug($"Add new match request {parseObj}");
            MatchRequests[matchReq.Id] = matchReq;
            RequestsQ.Enqueue(matchReq.Id);
        }

        //Update specific match request by id with new location
        public async Task UpdateMatchReq(int matchReqId, Location location)
        {
            var parseObj = JsonConvert.SerializeObject(location);
            Log.Debug($"Update match request {matchReqId} with {parseObj}");

            if (MatchRequests.ContainsKey(matchReqId))
            {
                MatchRequests[matchReqId].MatchReqDetails.Location = location;
                MatchRequests[matchReqId].Updated = DateTime.Now;
            }
            else
            {
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");
            }
        }

        //Remove specific match request from the dictionary only
        public async Task RemoveMatchReq(int matchReqId)
        {
            Log.Debug($"Remove match request {matchReqId}");
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
        public async Task SuspendMatchReq(int matchReqId)
        {
            Log.Debug($"Suspend match request {matchReqId}");
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
        public async Task RestoreMatchReq(int matchReqId)
        {
            Log.Debug($"Restore match request {matchReqId}");
            if (MatchRequests.ContainsKey(matchReqId))
            {
                MatchRequests[matchReqId].WaitForOptionalMatchRes = false;
                await RepositionMatchReq(matchReqId);
            }
            else
            {
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");
            }
        }

        //Enqueue the next match request for optional match calculation
        public MatchRequest GetNextMatchRequest()
        {
            if (RequestsQ.Count != 0)
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
        public async Task RepositionMatchReq(int matchReqId)
        {
            Log.Debug($"Reposition match request {matchReqId} on the Q");
            if (MatchRequests.ContainsKey(matchReqId))
            {
                RequestsQ.Enqueue(matchReqId);
            }
            else
            {
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");
            }
        }

        //Get match request object from the dictionary
        public MatchRequest GetMatchReqById(int matchReqId)
        {
            if (MatchRequests.ContainsKey(matchReqId))
            {
                return MatchRequests[matchReqId];
            }
            else
            {
                throw new MissingMatchRequestIdException($"match request id: {matchReqId} was not found");
            }
        }

        //Get Match request by ownerId return -1 if not found
        public int GetMatchReqIdByOwner(string userId)
        {
            var match = MatchRequests.FirstOrDefault(x => x.Value.MatchOwner == userId);

            return match.Value != null ? match.Value.Id : -1;
        }
    }
}