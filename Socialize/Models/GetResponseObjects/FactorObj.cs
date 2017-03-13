using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models.GetResponseObjects
{
    /*
     * Class object that contain specific factor 
     * later calculation
     */
    public class FactorObj
    {
        public string Class { get; set; }
        // List of sub-classes object under the class title
        public List<string> SubClasses { get; set; }
    }
}