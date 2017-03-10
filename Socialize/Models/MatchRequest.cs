﻿using System;
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
        // Type of the Match - one on one match / party
        public MatchType MatchType { get; set; }
        // Factores the should be calculated in order to find match
        public MatchReqDetails MatchReqDetails { get; set; }
        // True if found optional match - Match request suspended, False otherwise
        public bool WaitForOptionalMatchRes { get; set; }
    }

    public class MatchReqDetails
    {
        // List of factors that selected to be compared
        public List<Factor> MatchFactors { get; set; }
        // Location of the User who create the request
        public Location location { get; set; }
    }
}