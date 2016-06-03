using System;
using System.Threading.Tasks;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;

using Orleans;
using Orleans.Runtime.Configuration;

namespace Raft
{
    /// <summary>
    /// Orleans test silo host
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // The Orleans silo environment is initialized in its own app domain in order to more
            // closely emulate the distributed situation, when the client and the server cannot
            // pass data via shared memory.
            AppDomain hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitSilo,
                AppDomainInitializerArguments = args,
            });

            var runtime = PSharpRuntime.Create();
            Program.Execute(runtime);

            Console.WriteLine("Orleans Silo is running.\nPress Enter to terminate...");
            Console.ReadLine();

            hostDomain.DoCallBack(ShutdownSilo);
        }

        [Microsoft.PSharp.Test]
        public static void Execute(PSharpRuntime runtime)
        {
            Configuration conf = Configuration.Create(true, true, true, true, true);
            ActorModel.Configure(conf);
            
            ActorModel.Start(runtime, () =>
            {
                var config = ClientConfiguration.LocalhostSilo();
                GrainClient.Initialize(config);

                runtime.RegisterMonitor(typeof(SafetyMonitor));

                var sender = GrainClient.GrainFactory.GetGrain<IClusterManager>(0);
                Task configureTask = sender.Configure();
                ActorModel.Wait(configureTask);
            });
        }

        static void InitSilo(string[] args)
        {
            hostWrapper = new OrleansHostWrapper(args);

            if (!hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        static void ShutdownSilo()
        {
            if (hostWrapper != null)
            {
                hostWrapper.Dispose();
                GC.SuppressFinalize(hostWrapper);
            }
        }

        private static OrleansHostWrapper hostWrapper;
    }
}
