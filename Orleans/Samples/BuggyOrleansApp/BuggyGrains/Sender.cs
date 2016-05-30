using System;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;
using Orleans;

namespace BuggyOrleansApp
{
    /// <summary>
    /// Grain implementation class Server.
    /// </summary>
    public class Sender : Grain, ISender
    {
        public Task DoSomething(int numberOfItems)
        {
            var receiver = GrainClient.GrainFactory.GetGrain<IReceiver>(1);
            receiver.StartTransaction();
            for (int i = 0; i < numberOfItems; i++)
                receiver.TransmitData("xyz" + i);

            int transmitted = ActorModel.GetResult<int>(receiver.GetCurrentCount());
            ActorModel.Assert(transmitted <= numberOfItems, "Items sent: " + numberOfItems + "; Transmitted: " + transmitted);
            return Task.FromResult(true);
        }
    }
}
