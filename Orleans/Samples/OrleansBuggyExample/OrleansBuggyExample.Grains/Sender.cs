using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

using Orleans;

namespace OrleansBuggyExample
{
    /// <summary>
    /// Grain implementation class Sender.
    /// </summary>
    public class Sender : Grain, ISender
    {
        public Task DoSomething(int numberOfItems)
        {
            Console.WriteLine("DoSomething");
            var receiver = GrainClient.GrainFactory.GetGrain<IReceiver>(1);
            receiver.StartTransaction();
            for (int i = 0; i < numberOfItems; i++)
                receiver.TransmitData("xyz" + i);

            int transmitted = receiver.GetCurrentCount().Result;
            Console.WriteLine("Items sent: " + numberOfItems + "; Transmitted: " + transmitted);
            Contract.Assert(transmitted <= numberOfItems, "Items sent: " + numberOfItems + "; Transmitted: " + transmitted);
            return Task.FromResult(true);
        }
    }
}
