using Socialize.Models.GetResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * This folder holds all wrapper object that contain all data of post calls 
 */

namespace Socialize.Models.PostRequestObjects
{
    public class UpdateUserObj
    {
        public FactorObj[] Data { get; set; }
    }
}