using System;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

using ActorInterfaces;

namespace Actors
{
    public class Actor1 : Actor, IActor1
    {
        //protected override Task OnActivateAsync()
        //{
        //    (this as IActor1).Foo();
        //    return base.OnActivateAsync();
        //}

        public Task Foo()
        {
            var actor2Proxy = ActorProxy.Create<IActor2>(new ActorId(1), "A2");
            int val = 9;
            var t = actor2Proxy.SetValue(val, this);
            Console.WriteLine("Actor1 is waiting for set value from Actor2");
            ActorModel.Wait(t);

            Task<int> s = actor2Proxy.GetValue();
            var r = ActorModel.GetResult<int>(s);
            Console.WriteLine(r);

            return Task.FromResult(true);
        }

        public Task Bar()
        {
            return new Task(() => { });
        }
    }
}
