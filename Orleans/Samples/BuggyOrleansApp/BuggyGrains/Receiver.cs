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
    [Reentrant]
    public class Receiver : Grain<int>, IReceiver
    {
        public override async Task OnActivateAsync()
        {
            this.State = 0;
            await base.OnActivateAsync();
        }

        public Task<int> GetCurrentCount()
        {
            return Task.FromResult(this.State);
        }

        public async Task StartTransaction()
        {
            var sender = GrainClient.GrainFactory.GetGrain<ISender>(0);
            ActorModel.Assert(sender != null, "sender proxy is null");
            await sender.Dummy();
            this.State = 0;
            await this.WriteStateAsync();
        }

        public Task TransmitData(TransactionItems item)
        {
            Console.WriteLine(item.name);
            int count = this.State;
            count++;
            this.State = count;
            return this.WriteStateAsync();
        }
    }
}
