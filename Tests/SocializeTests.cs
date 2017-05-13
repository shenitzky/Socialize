using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Socialize.Models;
using Socialize.Logic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class SocializeTests
    {
        MatchManager manager = MatchManager.GetManagerInstance();
        MatchReqHandler handler = MatchReqHandler.GetMatchReqHandlerInstance(AlgorithemsTypes.IntuitiveMatchAlg);
        MatchReqContainer matchReqContainer = MatchReqContainer.GetMatchReqContainerInstance();
        OptionalMatchContainer optionalMatchContainer = OptionalMatchContainer.GetOptionalMatchContainerInstance();
        MatchRequest[] matchReqs;
        int optionalMatchId = -1;

        [TestInitialize()]
        public void Initialize()
        {
            matchReqs = CreateTwoMatchReq();
        }

        [TestMethod]
        public async Task AddMatchReq()
        {
            /*
            - Create 2 match requests
            - Add the match requests to the container
            - Expected Result - container size should be 2
            */

            await manager.CreateMatchRequest(matchReqs[0]);
            await manager.CreateMatchRequest(matchReqs[1]);

            var containerSize = matchReqContainer.GetAllOtherRequests(matchReqs[0].Id).Length + 1;
            Assert.AreEqual(containerSize, 2);
            await manager.RemoveMatchRequest(matchReqs[0].Id);
            await manager.RemoveMatchRequest(matchReqs[1].Id);
        }

        [TestMethod]
        public async Task AddOptionalMatch()
        {
            /*
            - Create 2 match requests
            - Add the match requests to the container
            - Create optional match with the match requests
            - Check if optional match added to the container
            - Expected Result - optional match shoul exsits in the container
            */
            matchReqs = CreateTwoMatchReq();
            await manager.CreateMatchRequest(matchReqs[0]);
            await manager.CreateMatchRequest(matchReqs[1]);

            OptionalMatchBuilder OptionalMatchBuilder = new OptionalMatchBuilder();
            var optionalMatch =  OptionalMatchBuilder.CreateOptionalMatch(
                matchReqs[0], matchReqs[1], new Dictionary<int, int>() { { matchReqs[0].Id, 100 }, { matchReqs[1].Id, 100 } }
                );
            optionalMatchId = optionalMatch.Id;

            optionalMatchContainer.AddOptionalMatch(optionalMatch);

            var resOptionalMatch = manager.GetOptionalMatchByOwnerId(matchReqs[0].MatchOwner);
            Assert.IsNotNull(resOptionalMatch);
            await manager.RemoveMatchRequest(matchReqs[0].Id);
            await manager.RemoveMatchRequest(matchReqs[1].Id);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            if(optionalMatchId != -1) manager.RemoveOptionalMatchById(optionalMatchId);
        }

        private MatchRequest[] CreateTwoMatchReq()
        {
            var imgUrl = "";
            var firstReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.1, lng = 0.1 },
                MatchFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "sport",
                       SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Soccer", ImgUrl = imgUrl },
                                new SubClass() { Name = "Basketball", ImgUrl = imgUrl },
                            }
                   },
                   new Factor()
                   {
                       Class = "gamming",
                       SubClasses = new List<SubClass>() { new SubClass() { Name = "ps4", ImgUrl = imgUrl } }
                   },
                   new Factor()
                   {
                       Class = "work",
                       SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Prog", ImgUrl = imgUrl },
                            }
                    }
                }
            };
            var secReq = new MatchReqDetails()
            {
                Location = new Location() { lat = 1.5, lng = 0.1 },
                MatchFactors = new List<Factor>()
               {
                   new Factor()
                   {
                       Class = "sport",
                       SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Soccer", ImgUrl = imgUrl },
                                new SubClass() { Name = "Basketball", ImgUrl = imgUrl },
                            }
                   },
                   new Factor()
                   {
                       Class = "gamming",
                       SubClasses = new List<SubClass>() { new SubClass() { Name = "ps4", ImgUrl = imgUrl } }
                   },
                   new Factor()
                   {
                       Class = "work",
                       SubClasses = new List<SubClass>()
                            {
                                new SubClass() { Name = "Prog", ImgUrl = imgUrl },
                            }
                    }
                }
            };

            var first = new MatchRequest();
            first.MatchReqDetails = firstReq;
            first.MatchOwner = "71b9ca3f-a85e-40db-a7f3-c4c3373a46b5";
            var sec = new MatchRequest();
            sec.MatchReqDetails = secReq;
            sec.MatchOwner = "74d64e5e-8ae2-4159-a492-a5b0bc0426a2";

            return new MatchRequest[] { first, sec };
        }
    }
}
