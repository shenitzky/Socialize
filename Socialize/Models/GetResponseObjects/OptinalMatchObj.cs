using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models.GetResponseObjects
{
    public class OptinalMatchObj
    {
        public DateTime Created { get; set; }
        public int Id { get; set; }
        public List<Factor> MatchedFactors { get; set; }
        public int MatchRequestId { get; set; }
        public int MatchStrength { get; set; }

        public UserDataObj MatchedDetails { get; set; }

        public string[] RawMatchFactors { get; set; }
    }
}