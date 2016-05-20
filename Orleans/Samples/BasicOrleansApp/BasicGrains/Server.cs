using System;
using System.Threading.Tasks;
using Orleans;

namespace BasicOrleansApp
{
    /// <summary>
    /// Grain implementation class Server.
    /// </summary>
    public class Server : Grain, IServer
    {
        public Task<int> Ping(IClient client, int counter)
        {
            Console.WriteLine("Server Pongs with " + counter);
            client.Pong(counter--);
            return Task.FromResult(counter--);
        }
    }
}
