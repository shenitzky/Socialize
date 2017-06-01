using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models.PostRequestObjects
{
    public class UserExtraData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
    }
}