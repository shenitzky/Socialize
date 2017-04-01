using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Socialize.Models;

namespace Socialize.Logic
{
    /*
     * Intuitive match algorithem, calculate match strength between two match requests
     */
    public class IntuitiveMatchAlg : IMatchAlg
    {
        public double MAX_DISTANCE { get; set; }

        public Dictionary<int, int> CalcOptionalMatch(MatchRequest first, MatchRequest sec)
        {
            //Holds the total subclasses that selected by the two requests
            var firstTotalSubClassesNum = 0;
            var secTotalSubClassesNum = 0;

            //Check proximity between the two requests
            var distance = SocializeUtil.CalculateLocationPriximity(
                first.MatchReqDetails.Location, sec.MatchReqDetails.Location
                );

            if(distance > MAX_DISTANCE)
            {
                return null;
            }

            var firstFactors = first.MatchReqDetails.MatchFactors;
            var secFactors = sec.MatchReqDetails.MatchFactors;

            var firstSum = 0.0;
            var secSum = 0.0;
            foreach (var factor in firstFactors)
            {
                var results = FindFactorMatch(factor, secFactors);
                firstSum += results.First();
                secSum += results.Last();
                firstTotalSubClassesNum += factor.SubClasses.ToArray().Length;
            }

            //Calculate the total number of sub-classes that selected by the second match request
            foreach(var factor in secFactors)
            {
                secTotalSubClassesNum += factor.SubClasses.ToArray().Length;
            }

            //Calculate the finale results of the match strength
            var finaleFirstRes = (int)((firstSum / firstTotalSubClassesNum) * 100);
            var finaleSecRes = (int)((secSum / secTotalSubClassesNum) * 100);

            return new Dictionary<int, int>() { { first.Id, finaleFirstRes }, { sec.Id, finaleSecRes } };
        }


        public List<double> FindFactorMatch(Factor factor, List<Factor> factors)
        {
            var firstSum = 0.0;
            var secSum = 0.0;

            //Check if there is a class match
            var FactorClassFound = factors.Where(x => x.Class.Equals(factor.Class)).FirstOrDefault();

            if (FactorClassFound != null)
            {
                //Multiplay the total subclasses that found by 0.5 to 
                //calculate the second match req strength
                var FactorSubClasses = FactorClassFound.SubClasses.ToArray();
                secSum += FactorSubClasses.Length * 0.5;
                firstSum += 0.5;

                foreach (var subclass in factor.SubClasses)
                {
                    var subclassFound = FactorSubClasses.Where(x => x.Equals(subclass)).FirstOrDefault();
                    if(subclassFound != null)
                    {
                        firstSum += 0.5;
                        secSum += 0.5;
                    }
                }
            }

            return new List<double>() { firstSum, secSum };

        }

    }
}