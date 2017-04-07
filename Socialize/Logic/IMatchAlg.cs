using Socialize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialize.Logic
{
    /*
     * Defin interface that should be implemented by all match algorithems
     */

    public interface IMatchAlg
    {
        /* Calculate what is the match strength between two match requests
         * Returns Dictionary<MatchRequestId, matchStrength> holds the strength of the match 
         * for each match request
         */

        //Specify the maximum distance between two different locations
        double MAX_DISTANCE { get; }


        Dictionary<int, int> CalcOptionalMatch(MatchRequest first, MatchRequest sec);
    }
}
