using System;
using System.Threading.Tasks;
using Orleans;

using Microsoft.PSharp.Actors;

namespace BuggyOrleansApp
{
    /// <summary>
    /// Grain implementation class Receiver.
    /// </summary>
    //[StorageProvider(ProviderName = "DevStore")]
    public class Receiver : Grain<int>, IReceiver
    {
        public override Task OnActivateAsync()
        {
            this.State = 0;
            this.WriteStateAsync().Wait();
            return base.OnActivateAsync();
        }

        public Task<int> GetCurrentCount()
        {
            this.ReadStateAsync().Wait();
            return Task.FromResult(this.State);
        }

        public Task StartTransaction()
        {
            //var sender = GrainClient.GrainFactory.GetGrain<ISender>(0);
            //ActorModel.Assert(sender != null, "sender proxy is null");
            //Task t = sender.Dummy();
            //ActorModel.Wait(t);
            this.State = 0;
            return this.WriteStateAsync();
        }

        public Task TransmitData(TransactionItems item)
        {
            Console.WriteLine(item.name);
            this.ReadStateAsync().Wait();
            int count = this.State;
            count++;
            this.State = count;
            return this.WriteStateAsync();
        }
    }
}
