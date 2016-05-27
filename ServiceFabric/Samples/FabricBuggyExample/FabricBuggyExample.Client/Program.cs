using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Sender.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabricBuggyExample.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var senderProxy = ActorProxy.Create<ISender>(new ActorId(0), "fabric:/FabricBuggyExample");
            Task t = senderProxy.DoSomething(10);
            t.Wait();
            Console.WriteLine("DONE!!!");
        }
    }
}
