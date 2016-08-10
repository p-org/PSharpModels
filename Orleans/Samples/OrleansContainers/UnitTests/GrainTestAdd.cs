using Microsoft.PSharp.Actors;
using Orleans;
using Orleans.Collections;
using Orleans.Collections.Utilities;
using Orleans.Streams;
using Orleans.Streams.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class GrainTestAdd : Grain, ITestAdd
    {
        public async Task startTest()
        {
            IStreamProvider _provider = base.GetStreamProvider("CollectionStreamProvider");

            var distributedCollection = GetRandomDistributedCollection<int>();
            await distributedCollection.SetNumberOfNodes(4);

            var l = Enumerable.Range(0, 20000).ToList();

            var references = await distributedCollection.BatchAdd(l);

            ActorModel.Assert(!(references.Any(item => item == null)), "Batch add failed!!");

            var consumer = new MultiStreamListConsumer<ContainerHostedElement<int>>(_provider);

            //Modified: split a statement with 2 awaits (AwaitRewriter must be fixed)
            //var getStrIds = await distributedCollection.GetStreamIdentities();
            //await consumer.SetInput(getStrIds);

            //var tid = await distributedCollection.EnumerateToStream();
            //await consumer.TransactionComplete(tid);

            //////CollectionAssert.AreEquivalent(l, consumer.Items.Select(x => x.Item).ToList());
            ////TODO Rashmi: Write equivalent Assert.
        }

        private IContainerGrain<T> GetRandomDistributedCollection<T>()
        {
            var grain = GrainFactory.GetGrain<IContainerGrain<T>>(Guid.NewGuid());

            return grain;
        }
    }
}
