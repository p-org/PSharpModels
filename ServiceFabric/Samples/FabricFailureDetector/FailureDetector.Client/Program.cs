using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FailureDetector.Interfaces;

namespace FailureDetector.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var senderProxy = ActorProxy.Create<IDriver>(new ActorId(0), "fabric:/FabricFailureDetector");
            Console.WriteLine("DONE!!!");
        }
    }
}
