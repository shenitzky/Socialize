using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Socialize.Models
{
    /*
     * DB Class object that contain Optional Match Object for Log and history
     */
    public class OptionalMatchLog
    {
        [Key]
        public int Id { get; set; }
        // Id of the User how owned the factor object
        public string UserId { get; set; }
        // Object with all the data on the optional match
        public DateTime Created { get; set; }
        // List of all the factor that calculated for the optional match
        public string MatchedFactors { get; set; }
        //Strength of the match for each user
        public string MatchStrength { get; set; }
    }
}