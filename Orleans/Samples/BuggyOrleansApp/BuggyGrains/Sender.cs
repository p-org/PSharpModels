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
    public class Sender : Grain, ISender, IRemindable
    {
        IGrainReminder myReminder;
        //public override Task OnActivateAsync()
        //{
        //    var reminderTask = this.RegisterOrUpdateReminder("helloReminder", TimeSpan.FromSeconds(2),
        //        TimeSpan.FromSeconds(0));
        //    ActorModel.Wait(reminderTask);
        //    myReminder = ActorModel.GetResult<IGrainReminder>(reminderTask);
        //    ////this.Timer = this.RegisterTimer(HandleTimeout, null,
        //    ////    TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0));
        //    return base.OnActivateAsync();
        //}

        public Task DoSomething(int numberOfItems)
        {
            var receiver = GrainClient.GrainFactory.GetGrain<IReceiver>(1);

            var task = receiver.StartTransaction();
            //ActorModel.Wait(task);

            for (int i = 0; i < numberOfItems; i++)
                receiver.TransmitData(new TransactionItems("xyz" + i));

            //int transmitted = ActorModel.GetResult<int>(receiver.GetCurrentCount());
            //ActorModel.Assert(transmitted <= numberOfItems, "Items sent: " + numberOfItems + "; Transmitted: " + transmitted);
            return Task.FromResult(true);
        }

        Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
        {
            UnregisterReminder(myReminder);
            var getReminderTask = GetReminders();
            var result = ActorModel.GetResult(getReminderTask);
            ActorModel.Assert(result.Count == 0, "number of reminders: " + result.Count);
            
            return Task.FromResult(true);
        }

        public Task Dummy()
        {
            return Task.FromResult(true);
        }

        //public Task HandleTimeout(object args)
        //{
        //    Console.WriteLine("Timed out");
        //    return Task.FromResult(true);
        //}
    }
}
