using System;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;
using Orleans;
using Orleans.Runtime;

namespace BuggyOrleansApp
{
    /// <summary>
    /// Grain implementation class Sender.
    /// </summary>
    public class Sender : Grain, ISender, IRemindable
    {
        public override Task OnActivateAsync()
        {
            this.RegisterOrUpdateReminder("helloReminder", TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(0));
            //this.Timer = this.RegisterTimer(HandleTimeout, null,
            //    TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0));
            return base.OnActivateAsync();
        }

        public Task DoSomething(int numberOfItems)
        {
            var receiver = GrainClient.GrainFactory.GetGrain<IReceiver>(1);
            receiver.StartTransaction();
            for (int i = 0; i < numberOfItems; i++)
                receiver.TransmitData(new TransactionItems("xyz" + i));

            int transmitted = ActorModel.GetResult<int>(receiver.GetCurrentCount());
            //ActorModel.Assert(transmitted <= numberOfItems, "Items sent: " + numberOfItems + "; Transmitted: " + transmitted);
            return Task.FromResult(true);
        }

        Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
        {
            Console.WriteLine("HELLO FROM REMINDER");
            return Task.FromResult(true);
        }

        //public Task HandleTimeout(object args)
        //{
        //    Console.WriteLine("Timed out");
        //    return Task.FromResult(true);
        //}
    }
}
