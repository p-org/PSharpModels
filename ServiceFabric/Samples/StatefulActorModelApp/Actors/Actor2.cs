using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PSharp;

using ActorInterfaces;
using Microsoft.PSharp.Actors;

using Microsoft.PSharp.Actors.Bridge;

namespace Actors
{
    public class Actor2 : Actor, IActor2
    {
        protected override Task OnActivateAsync()
        {
            this.StateManager.TryAddStateAsync("value", 0);
            return base.OnActivateAsync();
        }

        public Task<int> GetValue()
        {
            return this.StateManager.GetStateAsync<int>("value");
        }

        public Task SetValue(int val, IActor1 actor1Proxy)
        {
            var t = actor1Proxy.Bar();
            Console.WriteLine("Actor2 is waiting for bar from Actor1 ..");
            Console.WriteLine("?? " + (t is ActorCompletionTask<object>));
            ActorModel.Wait(t);
            return this.StateManager.SetStateAsync("value", val);
        }
    }
}
