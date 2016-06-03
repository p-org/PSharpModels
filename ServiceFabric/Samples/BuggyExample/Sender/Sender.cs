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
    public class Sender : Actor, ISender, IRemindable
    {
        IActorTimer myTimer;
        IActorReminder myReminder;
        protected override Task OnActivateAsync()
        {
            //myTimer = this.RegisterTimer(HandleTimeout, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0));
            Task<IActorReminder> rem = RegisterReminderAsync("helloReminder", null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            myReminder = ActorModel.GetResult(rem);
            return base.OnActivateAsync();
        }

        public Task DoSomething(int numberOfItems)
        {
            var receiverProxy = ActorProxy.Create<IReceiver>(new ActorId(1), "ReceiverProxy");
            receiverProxy.StartTransaction();
            for (int i = 0; i < numberOfItems; i++)
                receiverProxy.TransmitData("xyz" + i);

            int transmitted = ActorModel.GetResult<int>(receiverProxy.GetCurrentCount());
            //ActorModel.Assert(transmitted <= numberOfItems, "Items sent: " + numberOfItems + "; Transmitted: " + transmitted);
            return Task.FromResult(true);
        }

        public Task HandleTimeout(object args)
        {
            Console.WriteLine("TIMED OUT!!!!!!!!!!!!!!!!!!!!!!");
            UnregisterTimer(myTimer);
            return Task.FromResult(true);
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            Console.WriteLine("HELLO FROM REMINDER $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            UnregisterReminderAsync(myReminder);
            ActorModel.Assert(GetReminder("helloReminder") == null);
            return Task.FromResult(true);
        }
    }
}
