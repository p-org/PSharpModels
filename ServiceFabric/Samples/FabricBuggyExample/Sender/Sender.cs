using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Receiver.Interfaces;
using Sender.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sender
{
    public class Sender : Actor, ISender
    {
        public Task DoSomething(int numberOfItems)
        {
            this.RegisterTimer(HandleTimeout, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10));

            var receiverProxy = ActorProxy.Create<IReceiver>(new ActorId(1), "fabric:/FabricBuggyExample");
            receiverProxy.StartTransaction();
            for (int i = 0; i < numberOfItems; i++)
                receiverProxy.TransmitData("xyz" + i);

            int transmitted = receiverProxy.GetCurrentCount().Result;

            ActorEventSource.Current.ActorMessage(this, "Final Count: {0}", transmitted);

            Contract.Assert(transmitted <= numberOfItems, "Items sent: " + numberOfItems + "; Transmitted: " + transmitted);

            return Task.FromResult(true);
        }

        public Task HandleTimeout(object args)
        {
            ActorEventSource.Current.Message("Timed out");
            return Task.FromResult(true);
        }
    }
}
