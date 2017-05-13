using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Socialize.Models
{
    /*
     * DB Class object that contain Match Request Object for Log and history
     */
    public class MatchRequestLog
    {
        [Key]
        // Id of the request
        public int Id { get; set; }
        // Owner who created the match request
        public string UserId { get; set; }
        // Time and date when the request created
        public DateTime Created { get; set; }
        // Factores the should be calculated in order to find match
        public string MatchFactors { get; set; }
        // Location of the User who create the request
        public string Location { get; set; }
        //Distance selected for match
        public int maxDistance { get; set; }
    }
}