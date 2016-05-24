using ServiceFabricModel;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Example
{
    public class MyHuman : Actor, IHuman
    {
        protected override Task OnActivateAsync()
        {
            Console.WriteLine("Setting state");
            if (this.StateManager == null)
            {
                this.StateManager.TryAddStateAsync("name", "asdf");
            }
            return Task.FromResult(true);
        }

        public Task<int> Eat(int x, int y, string s)
        {
            Console.WriteLine("received: " + x + " " + y + " " + s);
            /*var obj = ActorProxy.Create<IAnotherHuman>(ActorId.NewId(), " ");
            Task<int> p = obj.Play(1000, 6, "gotit");
            Task<int> q = obj.Play(999, 8, "asdf");*/
            //Console.WriteLine(GetResult(p));
            return Task.FromResult(x + 7);
        }
        
        public int GetResult(Task<int> t)
        {
            return t.Result;
        }
    }
}
