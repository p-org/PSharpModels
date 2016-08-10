using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Receiver.Interfaces;
using Sender.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionData;

namespace Sender
{
    public class Sender : Actor, ISender
    {
        protected override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
        }

        public Task DoSomething(int numberOfItems)
        {
            var receiverProxy = ActorProxy.Create<IReceiver>(new ActorId(1), "ReceiverProxy");
            receiverProxy.StartTransaction();
            for (int i = 0; i < numberOfItems; i++)
                receiverProxy.TransmitData("xyz" + i);
            
            int transmitted = ActorModel.GetResult<int>(receiverProxy.GetCurrentCount());
            ActorModel.Assert(transmitted <= numberOfItems, "Items sent: " + numberOfItems + "; Transmitted: " + transmitted);

            return Task.FromResult(true);
        }
    }
}
