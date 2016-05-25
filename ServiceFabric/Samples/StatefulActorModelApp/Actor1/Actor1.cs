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
            actor2Proxy.Wait(t);

            Task<int> s = actor2Proxy.GetValue();
            var r = actor2Proxy.GetResult<int>(s);
            Console.WriteLine(r);

            return Task.FromResult(true);
        }

        public void Wait<TResult>(Task<TResult> task)
        {
            throw new NotImplementedException();
        }

        TResult IActor1.GetResult<TResult>(Task<TResult> task)
        {
            throw new NotImplementedException();
        }

        void IActor1.Wait(Task task)
        {
            throw new NotImplementedException();
        }

        //public int GetResult(Task<int> t)
        //{
        //    //return t.Result;
        //    return (int)((ServiceFabricModel.FabricActorMachine)this.RefMachine).GetResult(t);
        //}
    }
}
