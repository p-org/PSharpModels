using Microsoft.ServiceFabric.Actors.Runtime;
using Receiver.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receiver
{
    public class Receiver : Actor, IReceiver
    {
        protected override Task OnActivateAsync()
        {
            this.StateManager.AddStateAsync<int>("itemCount", 0);
            return base.OnActivateAsync();
        }

        public Task<int> GetFinalCount()
        {
            return Task.FromResult(this.StateManager.GetStateAsync<int>("itemCount").Result);
        }

        public Task StartTransaction()
        {
            return this.StateManager.SetStateAsync<int>("itemCount", 0);
        }

        public Task TransmitData(string item)
        {
            ActorEventSource.Current.ActorMessage(this, "Received item: {0}", item);
            int count = this.StateManager.GetStateAsync<int>("itemCount").Result;
            count++;
            this.StateManager.SetStateAsync<int>("itemCount", count);
            return Task.FromResult(true);
        }
    }
}
