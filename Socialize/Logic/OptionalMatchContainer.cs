using log4net;
using Newtonsoft.Json;
using Socialize.Exeptions;
using Socialize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Logic
{
    public class OptionalMatchContainer
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Dictionary<int, IOptionalMatch> OptionalMatches { get; private set; }

        //singlton implementation
        private static OptionalMatchContainer OptionalContainerInstance;

        public static OptionalMatchContainer GetOptionalMatchContainerInstance()
        {
            if (OptionalContainerInstance == null)
            {
                OptionalContainerInstance = new OptionalMatchContainer();
            }
            return OptionalContainerInstance;
        }

        private OptionalMatchContainer()
        {
            OptionalMatches = new Dictionary<int, IOptionalMatch>();
        }

        //Add new optional match to the dictionary
        public void AddOptionalMatch(IOptionalMatch newOptionalMatch)
        {
            var parseObj = JsonConvert.SerializeObject(newOptionalMatch);
            Log.Debug($"Add Optional match {parseObj}");

            OptionalMatches[newOptionalMatch.Id] = newOptionalMatch;
        }

        //Remove Optional match by match request ID
        public void RemoveOptionalMatchByMatchReqId(int matchReqId)
        {
            var optionalMatchToRemove = GetOptionalMatchByMatchRequestId(matchReqId).Id;
            OptionalMatches.Remove(optionalMatchToRemove);
        }

        //Remove Optional match by optional match ID
        public void RemoveOptionalMatchByOptionalMatchId(int optionalMatchId)
        {
            Log.Debug($"Remove Optional match by optional match Id {optionalMatchId}");
            if (OptionalMatches.ContainsKey(optionalMatchId))
            {
                OptionalMatches.Remove(optionalMatchId);
            }
        }

        //Get optional match by match request id
        public IOptionalMatch GetOptionalMatchByMatchRequestId(int matchReqId)
        {
            var optionalMatch = OptionalMatches.FirstOrDefault(x => x.Value.MatchRequestIds.Any(z => z == matchReqId));
            return optionalMatch.IsDefault() ? null : optionalMatch.Value;
        }

        //Get optional match by optional match id
        public IOptionalMatch GetOptionalMatchByOptionalMatchId(int optionalMatchId)
        {
            return OptionalMatches.ContainsKey(optionalMatchId) ? OptionalMatches[optionalMatchId] : null;
        }
        //Update final match received for match id
        public void SetFinalMatchReceivedForOptionalMatch(int optionalMatchId, int matchReqId)
        {
            Log.Debug($"Set final match Received for optionalMatch id={optionalMatchId} and match req id= {matchReqId}");

            if (OptionalMatches.ContainsKey(optionalMatchId))
            {
                OptionalMatches[optionalMatchId].FinalMatchReceivedStatus[matchReqId] = true;
                return;
            }
            throw new MissingOptionalMatchException($"Can not find optional match id: {optionalMatchId}");
        }
        //Check if final match received for each user
        public bool CheckIfFinalMatchReceived(int optionalMatchId)
        {

            if (OptionalMatches.ContainsKey(optionalMatchId))
            {
                return (OptionalMatches[optionalMatchId].FinalMatchReceivedStatus.First().Value) && (OptionalMatches[optionalMatchId].FinalMatchReceivedStatus.Last().Value); 
                
            }
            throw new MissingOptionalMatchException($"Can not find optional match id: {optionalMatchId}");
        }

        //Update optional match with user accepte/decline by match request id
        public void UpdateOptionalMatch(int optionalMatchId, int matchReqId, bool status)
        {
            Log.Debug($"Update Optional match id: {optionalMatchId} status to {status} of match req id: {matchReqId}");

            if (OptionalMatches.ContainsKey(optionalMatchId))
            {
                OptionalMatches[optionalMatchId].Status[matchReqId] = status;
                return;
            }
            throw new MissingOptionalMatchException($"Can not find optional match id: {optionalMatchId}");
        }
    }
}