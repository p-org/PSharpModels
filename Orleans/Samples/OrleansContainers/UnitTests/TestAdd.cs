using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Orleans;
using Orleans.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    class TestAdd
    {
        static void Main(string[] args)
        {
            var runtime = PSharpRuntime.Create();
            TestAdd.Execute(runtime);

            Console.WriteLine("Unit test is running.\nPress Enter to terminate...");
            Console.ReadLine();
        }


        [Microsoft.PSharp.Test]
        public static void Execute(PSharpRuntime runtime)
        {
            Configuration conf = Configuration.Create(true, true, true, true, true);
            ActorModel.Configure(conf);
            ActorModel.Start(runtime, () =>
            {
                var tester = GrainClient.GrainFactory.GetGrain<ITestAdd>(0);
                Task t = tester.startTest();
            });
        }
    }
}
