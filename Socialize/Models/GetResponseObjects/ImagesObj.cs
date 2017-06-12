using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models.GetResponseObjects
{
    public class ImagesObj
    {
        public string First { get; set; }
        public string Sec { get; set; }

        public ImagesObj(string first, string sec)
        {
            this.First = first;
            this.Sec = sec;
        }
    }
}