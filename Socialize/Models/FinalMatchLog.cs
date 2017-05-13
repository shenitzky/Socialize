using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Socialize.Models
{
    /*
     * DB Class object that contain information on final match that calculated in the system
     */
    public class FinalMatchLog
    {
        // Id of the final match object
        [Key]
        public int Id { get; set; }
        // Users Id that participate in the match
        public string UsersId { get; set; }
        // Time-stamp
        public DateTime Created { get; set; }
        // List of the match strength - value for each user
        public string MatchStrength { get; set; }
        // Location where the match occured
        public string Locations { get; set; }
        // True if the match accepted, False otherwise
        public bool IsAccepted { get; set; }
        // Factor that calculated in the match
        public string Factors { get; set; }
        
    }

   
}