using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * This folder holds all wrapper object that contain all data for get calls 
 */

namespace Socialize.Models.GetResponseObjects
{
    public class UserDataObj
    {
        public string Id { get; set; }
        public string Mail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] Description { get; set; }
        public string ImgUrl { get; set; }
        public int Age { get; set; }
        public bool Premium { get; set; }
        public Factor[] Factors { get; set; }
    }
}