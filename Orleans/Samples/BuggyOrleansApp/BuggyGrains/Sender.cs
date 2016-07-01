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
    [Reentrant]
    public class Sender : Grain, ISender//, IRemindable
    {
        //IGrainReminder myReminder;
        //public override async Task OnActivateAsync()
        //{
            //var reminderTask = this.RegisterOrUpdateReminder("helloReminder", TimeSpan.FromSeconds(2),
            //    TimeSpan.FromSeconds(0));
            //ActorModel.Wait(reminderTask);
            //myReminder = ActorModel.GetResult<IGrainReminder>(reminderTask);
            ////this.Timer = this.RegisterTimer(HandleTimeout, null,
            ////    TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0));
        //    await base.OnActivateAsync();
        //}

        public async Task DoSomething(int numberOfItems)
        {
            var receiver = GrainClient.GrainFactory.GetGrain<IReceiver>(1);

            await receiver.StartTransaction();

            for (int i = 0; i < numberOfItems; i++)
                await receiver.TransmitData(new TransactionItems("xyz" + i));

            int transmitted = await receiver.GetCurrentCount();
            ActorModel.Assert(transmitted <= numberOfItems, "Items sent: " + numberOfItems + "; Transmitted: " + transmitted);
        }

        //async Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
        //{
        //    await UnregisterReminder(myReminder);
        //    var result = await GetReminders();
        //    ActorModel.Assert(result.Count == 0, "number of reminders: " + result.Count);
        //}

        public Task Dummy()
        {
            return Task.FromResult(true);
        }

        public Task HandleTimeout(object args)
        {
            Console.WriteLine("Timed out");
            return Task.FromResult(true);
        }
    }
}
