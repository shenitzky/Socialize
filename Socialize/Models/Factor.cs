using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Socialize.Models
{
    /*
     * DB Class object that contain specific factor of specific user for 
     * later calculation
     */
    public class Factor
    {
        [Key]
        public int Id { get; set; }
        // Id of the User how owned the factor object
        public string UserId { get; set; }
        // Class of the factor
        public string Class { get; set; }
        // List of sub-classes object under the class title
        public ICollection<SubClass> SubClasses { get; set; }
    }
}