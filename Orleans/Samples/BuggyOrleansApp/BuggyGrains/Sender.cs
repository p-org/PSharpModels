using System;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;
using Orleans;

namespace BuggyOrleansApp
{
    /// <summary>
    /// Grain implementation class Sender.
    /// </summary>
    public class Sender : Grain, ISender
    {
        //public override Task OnActivateAsync()
        //{
        //    this.Timer = this.RegisterTimer(HandleTimeout, null,
        //        TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0));
        //    return base.OnActivateAsync();
        //}

        public Task DoSomething(int numberOfItems)
        {
            var receiver = GrainClient.GrainFactory.GetGrain<IReceiver>(1);
            receiver.StartTransaction();
            for (int i = 0; i < numberOfItems; i++)
                receiver.TransmitData(new TransactionItems("xyz" + i));

            int transmitted = ActorModel.GetResult<int>(receiver.GetCurrentCount());
            ActorModel.Assert(transmitted <= numberOfItems, "Items sent: " + numberOfItems + "; Transmitted: " + transmitted);
            return Task.FromResult(true);
        }

        //public Task HandleTimeout(object args)
        //{
        //    Console.WriteLine("Timed out");
        //    return Task.FromResult(true);
        //}
    }
}
