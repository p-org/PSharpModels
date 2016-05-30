using System;
using System.Threading.Tasks;
using Orleans;

namespace BuggyOrleansApp
{
    /// <summary>
    /// Grain implementation class Client.
    /// </summary>
    public class Receiver : Grain, IReceiver
    {
        public override Task OnActivateAsync()
        {
            this.StateManager.AddStateAsync<int>("itemCount", 0);
            return base.OnActivateAsync();
        }

        public Task<int> GetCurrentCount()
        {
            return Task.FromResult(this.StateManager.GetStateAsync<int>("itemCount").Result);
        }

        public Task StartTransaction()
        {
            return this.StateManager.SetStateAsync<int>("itemCount", 0);
        }

        public Task TransmitData(string item)
        {
            Console.WriteLine(item);
            int count = this.StateManager.GetStateAsync<int>("itemCount").Result;
            count++;
            this.StateManager.SetStateAsync<int>("itemCount", count);
            return Task.FromResult(true);
        }
    }
}
