using Socialize.Models.GetResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialize.Models
{
    /*
     * Interface that define common data and methuds for OptionalMatch object 
     */
    public interface IOptionalMatch
    {
        // Id of the created optional match
        int Id { get; set; }
        // Time-stamp
        DateTime Created  { get; set; }
        // Status of the optional match - 
        // for each match request id (int) define if the optional match accepted or declined (bool)
        Dictionary<int, bool> Status { get; set; }
        // Status of the optional match -
        // for each match request id (int) define if the final match received(bool)
        Dictionary<int, bool> FinalMatchReceivedStatus { get; set; }
        // The strength of the optional match for each participant
        Dictionary<int, int> MatchStrength { get; set; }
        // The Id of each match request that take part in the optional match suggestion
        List<int> MatchRequestIds { get; set; }
        // The factors that calculated in the optional match
        List<Factor> MatchedFactors { get; set; }

        UserDataObj MatchedUser { get; set; }
    }
}
