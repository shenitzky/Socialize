using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Socialize.Logic
{
    public class StartThreadHandler
    {

        public static readonly Lazy<object> workerFactory = new Lazy<object>(() => {
            RunThread();
            return null;
        });

        public static async Task RunThread()
        {
            Func<Task> func = async () =>
            {
                var handler = MatchReqHandler.GetMatchReqHandlerInstance(AlgorithemsTypes.IntuitiveMatchAlg);
                int i = 0;
                while (true)
                {
                    Trace.WriteLine($"{i} before");
                    try
                    {
                        await handler.SendMatchReqToFindMatch();
                    }
                    catch(Exception ex)
                    {
                        int z = 90;
                    }
                    
                    Thread.Sleep(3000);
                    Trace.WriteLine($"{i++} after");
                }
            };
            Thread oThread = new Thread(new ThreadStart(async () => await func()));
            oThread.Start();
        }

        private static async Task ThreadLogic()
        {
            var handler = MatchReqHandler.GetMatchReqHandlerInstance(AlgorithemsTypes.IntuitiveMatchAlg);
            int i = 0;
            while (true)
            {
                Trace.WriteLine($"{i} before");
                await handler.SendMatchReqToFindMatch();
                Thread.Sleep(3000);
                Trace.WriteLine($"{i++} after");
            }
            
        }
    }
}