using System;
using System.Threading.Tasks;
using Orleans;

namespace BasicOrleansApp
{
    /// <summary>
    /// Grain implementation class Client.
    /// </summary>
    public class Client : Grain, IClient
    {
        private IServer Server;
        private int Counter;

        public override Task OnActivateAsync()
        {
            this.Counter = 10;
            Console.WriteLine("Client: activated");
            return base.OnActivateAsync();
        }

        public Task<string> Initialize(IServer server)
        {
            this.Server = server;
            this.Counter = 10;
            Console.WriteLine("Client: initializing");
            return Task.FromResult("Done");
        }

        public Task<int> Ping()
        {
            for (int idx = 0; idx < 10; idx++)
            {
                Console.WriteLine("Client PINGs with " + idx);
                this.Server.Ping(this, idx);
            }

            return Task.FromResult(this.Counter);
        }

        public Task<int> Pong(int counter)
        {
            Console.WriteLine("Client received " + counter);
            this.Counter = counter;
            return Task.FromResult(this.Counter);
        }
    }
}
