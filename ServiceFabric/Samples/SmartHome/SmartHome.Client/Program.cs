using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartHome.Interfaces;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;

namespace FabricBuggyExample.Client
{
    class Program
    {
        public static void Main(String[] args)
        {
            var runtime = PSharpRuntime.Create();
            Program.Execute(runtime);
            Console.ReadLine();
        }

        [Microsoft.PSharp.Test]
        public static void Execute(PSharpRuntime runtime)
        {
            ActorModel.Configure(Configuration.Create(false, false, false, false, false));

            ActorModel.Start(runtime, () =>
            {
                var environment = ActorProxy.Create<IEnvironment>(new ActorId(0), "fabric:/FabricSmartHome");
                Task t = environment.Start();
                ActorModel.Wait(t);
            });
        }
    }
}
