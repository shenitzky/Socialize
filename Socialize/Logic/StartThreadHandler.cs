using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

namespace Socialize.Logic
{
    public class StartThreadHandler
    {

        public static readonly Lazy<object> workerFactory = new Lazy<object>(() => {
            RunThread();
            return null;
        });

        public static void RunThread()
        {
            Thread oThread = new Thread(new ThreadStart(ThreadLogic));
            oThread.Start();
            //while (!oThread.IsAlive) ;

        }

        private static void ThreadLogic()
        {
            var handler = MatchReqHandler.GetMatchReqHandlerInstance(AlgorithemsTypes.IntuitiveMatchAlg);
            int i = 0;
            while (true)
            {
                Trace.WriteLine($"{i} before");
                handler.SendMatchReqToFindMatch();
                Thread.Sleep(3000);
                Trace.WriteLine($"{i++} after");
            }
            
        }
    }
}