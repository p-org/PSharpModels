using StatefulActorModel.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.PSharp.Utilities;
using Microsoft.PSharp.SystematicTesting;
using Microsoft.PSharp;

namespace StatefulActorModelApp.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ActorId id = ActorId.NewId();
            var proxy = ActorProxy.Create<IStatefulActorModel>(id, "fabric:/StatefulActorModelApp");

            int count = 10;

            for (int i = 0; i < 5; i++)
            {
                proxy.SetCountAsync(count);
                Console.WriteLine("Count from Actor {0}", proxy.GetCountAsync().Result);
                count++;
            }

            /*
            var proxy1 = ActorProxy.Create<IStatefulActorModel>(id, "fabric:/StatefulActorModelApp");
            Console.WriteLine(proxy.Equals(proxy1));
            */
        }
    }
}
