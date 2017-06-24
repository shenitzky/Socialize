using Socialize.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models
{
    /*
     * Class object that contain information about request to find new match
     */
    public class MatchRequest
    {
        // Id of the request
        public int Id { get; set; }
        // Owner who created the match request
        public string MatchOwner { get; set; }
        // Time and date when the request created
        public DateTime Created { get; set; }
        // Time and date when the request location updated
        public DateTime Updated { get; set; }
        // Type of the Match - one on one match / party
        public MatchType MatchType { get; set; }
        // Factores the should be calculated in order to find match
        public MatchReqDetails MatchReqDetails { get; set; }
        // True if found optional match - Match request suspended, False otherwise
        public bool WaitForOptionalMatchRes { get; set; }
        //Ignore list - all the declined match requests saves to prevent rematch
        public List<int> IgnoreList { get; set; }

        public MatchRequest()
        {
            this.Id = SocializeUtil.GeneratId();
            this.WaitForOptionalMatchRes = false;
            this.Created = DateTime.Now;
            this.Updated = DateTime.Now;
            this.IgnoreList = new List<int>();

            //default one on one match request - BETA
            this.MatchType = MatchType.ONE_TO_ONE;
        }
    }



    public class MatchReqDetails
    {
        // List of factors that selected to be compared
        public List<Factor> MatchFactors { get; set; }
        // Location of the User who create the request
        public Location Location { get; set; }

        public int maxDistance { get; set; }

        public int minMatchStrength { get; set; }
    }
}