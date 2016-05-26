﻿using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;
using Sender.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ActorModel.Start(runtime, () =>
            {
                var senderProxy = ActorProxy.Create<ISender>(new ActorId(0), "SenderProxy");
                Task t = senderProxy.DoSomething(10);
                ActorModel.Wait(t);
            });
        }
    }
}
