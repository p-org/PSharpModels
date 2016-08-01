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
            //IStreamProvider _provider = GrainClient.GetStreamProvider("CollectionStreamProvider");
            IStreamProvider _provider = base.GetStreamProvider("CollectionStreamProvider");

            var distributedCollection = GetRandomDistributedCollection<int>();
            await distributedCollection.SetNumberOfNodes(4);

            var l = Enumerable.Range(0, 20000).ToList();

            var references = await distributedCollection.BatchAdd(l);

            ////CollectionAssert.AllItemsAreNotNull(references);
            ActorModel.Assert(!(references.Any(item => item == null)));
            // TODO reference sanity check: Should range form 0 to 20000

            var consumer = new MultiStreamListConsumer<ContainerHostedElement<int>>(_provider);
            await consumer.SetInput(await distributedCollection.GetStreamIdentities());

            //var tid = await distributedCollection.EnumerateToStream();
            //await consumer.TransactionComplete(tid);

            ////CollectionAssert.AreEquivalent(l, consumer.Items.Select(x => x.Item).ToList());
            //foreach (var item in consumer.Items.Select(x => x.Item))
            //{
            //    ActorModel.Assert(item == 1);
            //}
        }

        private IContainerGrain<T> GetRandomDistributedCollection<T>()
        {
            var grain = GrainFactory.GetGrain<IContainerGrain<T>>(Guid.NewGuid());

            return grain;
        }
    }
}
