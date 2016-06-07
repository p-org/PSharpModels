using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartHome.Interfaces;

namespace FabricBuggyExample.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var environment = ActorProxy.Create<IEnvironment>(new ActorId(0), "fabric:/FabricSmartHome");
            Task t = environment.Start();
            t.Wait();
            Console.WriteLine("DONE!!!");
        }
    }
}
