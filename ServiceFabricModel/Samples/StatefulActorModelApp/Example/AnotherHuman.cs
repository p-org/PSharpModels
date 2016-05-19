using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Example
{
    public class AnotherHuman : Actor<HumanState>, IAnotherHuman
    {
        protected override Task OnActivateAsync()
        {
            Console.WriteLine("Setting state Play");
            if (this.State == null)
            {
                this.State = new HumanState();
                this.State.name = "asdf";
            }
            return Task.FromResult(true);
        }

        Task<int> IAnotherHuman.Play(int x, int y, string s)
        {
            Console.WriteLine("Playing: " + x + " " + y);
            return Task.FromResult(x + 10);
        }
    }
}
