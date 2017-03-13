﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Models.GetResponseObjects
{
    public class OptinalMatchObj
    {
        public DateTime Created { get; set; }
        public int Id { get; set; }
        public List<string> MatchedFactors { get; set; }
        public int MatchRequestId { get; set; }
        public int MatchStrength { get; set; }
    }
}