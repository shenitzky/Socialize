using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models
{
    /*
     * Class object that contain information about optional match 
     * that found between party different match requests
     */
    public class PartyOptionalMatch: IOptionalMatch
    {
        public DateTime Created { get; set; }
        public int Id { get; set; }
        public List<string> MatchedFactors { get; set; }
        public List<int> MatchRequestIds { get; set; }
        public List<int> MatchStrength { get; set; }
        public Dictionary<int, bool> Status { get; set; }
    }
}