using Actor2.Interfaces;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PSharp;

namespace Actor2
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

        public Task SetValue(int val)
        {
            return this.StateManager.SetStateAsync("value", val);
        }

        TResult IActor2.GetResult<TResult>(Task<TResult> task)
        {
            throw new NotImplementedException();
        }

        void IActor2.Wait(Task task)
        {
            throw new NotImplementedException();
        }

        public void Wait<TResult>(Task<TResult> task)
        {
            throw new NotImplementedException();
        }
    }
}
