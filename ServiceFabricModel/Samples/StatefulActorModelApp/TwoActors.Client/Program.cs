using Actor1.interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoActors.Client
{
    public class Program
    {
        public static void Main(String[] args)
        {
            var actor1Proxy = ActorProxy.Create<IActor1>(new ActorId(2), "A1");
            actor1Proxy.Foo();
            Console.ReadLine();
        }
    }
}
