using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * This folder holds all wrapper object that contain all data of post calls 
 */


namespace Socialize.Models.PostRequestObjects
{
    //Object holds match request update id and location
    public class MatchReqUpdateObj
    {
        public int matchReqId { get; set; }
        public Location location { get; set; }
    }
}