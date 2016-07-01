using System;
using System.Threading.Tasks;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;

using Sender.Interfaces;

namespace BuggyExample
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
            ActorModel.Start(runtime, async () =>
            {
                var senderProxy = ActorProxy.Create<ISender>(new ActorId(0), "SenderProxy");
                Task t = senderProxy.DoSomething(10);
                await t;
                //ActorModel.Wait(t);
            });
        }
    }
}
