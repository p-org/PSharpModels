using Actor1.interfaces;
using Actor2.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PSharp;

namespace Actor1
{
    public class Actor1 : Actor, IActor1
    {
        public Task Foo()
        {
            var actor2Proxy = ActorProxy.Create<IActor2>(new ActorId(1), "A2");
            int val = 9;
            var t = actor2Proxy.SetValue(val);
            Task<int> s = actor2Proxy.GetValue();
            var r = GetResult(s);
            Console.WriteLine(r);
            return Task.FromResult(true);
        }

        public int GetResult(Task<int> t)
        {
            //return t.Result;
            return (int)((ServiceFabricModel.FabricActorMachine)this.RefMachine).GetResult(t);
        }
    }
}
