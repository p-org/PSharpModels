using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using ActorInterfaces;

namespace TwoActors.Client
{
    public class Program
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
            ActorModel.Start(runtime, () =>
            {
                var actor1Proxy = ActorProxy.Create<IActor1>(new ActorId(2), "A1");
                actor1Proxy.Foo();
            });
        }
    }
}
