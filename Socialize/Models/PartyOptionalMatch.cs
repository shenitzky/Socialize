using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models
{
    public class PartyOptionalMatch: IOptionalMatch
    {
        public DateTime Created { get; set; }
        public int Id { get; set; }
        public MatchReqDetails[] MatchedUsersDetails { get; set; }
        public List<int> MatchRequestIds { get; set; }
        public List<int> MatchStrength { get; set; }
        public Dictionary<int, bool> Status { get; set; }
    }
}