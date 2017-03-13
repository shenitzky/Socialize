using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Socialize.Models
{
    /*
     * Class object that represents Location quardinants 
     */
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}