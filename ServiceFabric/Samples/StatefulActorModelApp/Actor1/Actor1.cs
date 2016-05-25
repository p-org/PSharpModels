using System;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

using Actor1.interfaces;
using Actor2.Interfaces;

namespace Actor1
{
    public class Actor1 : Actor, IActor1
    {
        public Task Foo()
        {
            var actor2Proxy = ActorProxy.Create<IActor2>(new ActorId(1), "A2");
            int val = 9;
            var t = actor2Proxy.SetValue(val);
            ActorModel.Wait(t);

            Task<int> s = actor2Proxy.GetValue();
            var r = ActorModel.GetResult<int>(s);
            Console.WriteLine(r);

            return Task.FromResult(true);
        }
    }
}
