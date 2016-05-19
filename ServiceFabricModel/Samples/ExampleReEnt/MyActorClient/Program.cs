using Microsoft.ServiceFabric.Actors;
using MyActor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyActorClient
{
    class Program
    {
        private static readonly string serviceName = "fabric:/TestReEnt";
        static void Main(string[] args)
        {
            ActorId id = new ActorId(1);
            var proxyA = ActorProxy.Create<IMyActor>(id, serviceName);
            var proxyB = ActorProxy.Create<IMyActor>(id, serviceName);
            //var proxyC = ActorProxy.Create<IMyActor>(new ActorId(3), serviceName);
            //var proxyD = ActorProxy.Create<IMyActor>(new ActorId(4), serviceName);

            int count = 10;
            //Faults();
            
            proxyA.SetCountAsync(count, new List<IMyActor>()).Wait();
            proxyB.SetCountAsync(count, new List<IMyActor>()).Wait();
            //proxyC.SetCountAsync(count, new List<IMyActor>()).Wait();
            //proxyD.SetCountAsync(count, new List<IMyActor>()).Wait();
            /*
            for (; count < 1000; count += 10)
            {*/
                var t1 = proxyA.SetCountAsync(count, new List<IMyActor> { proxyB, /*proxyC,*/ proxyA }).ContinueWith(t => { Console.WriteLine("Task t1 done"); });
                //var t2 = proxyD.SetCountAsync(count + 4, new List<IMyActor> { proxyA, proxyB }).ContinueWith(t => { Console.WriteLine("Task t2 done"); });
                t1.Wait();
                //t2.Wait();

                Console.WriteLine("Proxy A: {0}", proxyA.GetCountAsync().Result % 10);
                Console.WriteLine("Proxy B: {0}", proxyB.GetCountAsync().Result % 10);
            /*}*/
            Console.ReadLine();
        }

        /*
        private static async Task Faults()
        {
            TimeSpan maxServiceStabilizationTimeout = TimeSpan.FromSeconds(180);
            PartitionSelector randomPartitionSelector = PartitionSelector.RandomOf(new Uri(serviceName));

            var fabricClient = new FabricClient();
            TimeSpan timeToRun = TimeSpan.FromMinutes(60);
            FailoverTestScenarioParameters scenarioParameters = new FailoverTestScenarioParameters(
              randomPartitionSelector,
              timeToRun,
              maxServiceStabilizationTimeout);

            // Create the scenario class and execute it asynchronously.
            FailoverTestScenario failoverScenario = new FailoverTestScenario(fabricClient, scenarioParameters);

            try
            {
                await failoverScenario.ExecuteAsync(CancellationToken.None);
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }

        }
        */

    }
}

