using System;
using System.Threading.Tasks;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;

namespace FailureDetector
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
            Configuration conf = Configuration.Create(true, true, false, false, true);
            ActorModel.Configure(conf);

            ActorModel.Start(runtime, () =>
            {
                runtime.RegisterMonitor(typeof(SafetyMonitor));
                var senderProxy = ActorProxy.Create<IDriver>(new ActorId(0), "DriverProxy");
            });
        }
    }
}
