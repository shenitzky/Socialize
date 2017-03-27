using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Logic
{
    /*
     * Class implements the factory design pattern, return specific match algorithem
     */
    public class MatchAlgFactory
    {
        // Enums of all algorithem types that the factory can create
        public enum AlgorithemsTypes
        {
            IntuitiveMatchAlg,
        }

        public static IMatchAlg GetMatchAlg(AlgorithemsTypes algType)
        {
            switch (algType)
            {
                case AlgorithemsTypes.IntuitiveMatchAlg:
                    return new IntuitiveMatchAlg();

                default:
                    return new IntuitiveMatchAlg();
            }
        }
    }


    
}