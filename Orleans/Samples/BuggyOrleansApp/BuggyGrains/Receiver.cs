using System;
using System.Threading.Tasks;
using Orleans;

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
            this.State = 0;
            return this.WriteStateAsync();
        }

        public Task TransmitData(string item)
        {
            Console.WriteLine(item);
            this.ReadStateAsync().Wait();
            int count = this.State;
            count++;
            this.State = count;
            return this.WriteStateAsync();
        }
    }
}
