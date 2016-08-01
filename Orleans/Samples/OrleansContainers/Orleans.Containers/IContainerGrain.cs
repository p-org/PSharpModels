using System;
using System.Threading.Tasks;
using Orleans.Streams;
using Orleans;

namespace Orleans.Collections
{
    public interface IContainerGrain<T> : IGrainWithGuidKey, ICollectionOperations<T>, IBatchWriteable<T>, IElementExecutor<T>, IStreamProcessorAggregate<T, ContainerHostedElement<T>>
    {
        /// <summary>
        /// Sets the number of containers to use for this collection. Only increase of size is supported.
        /// </summary>
        /// <param name="numContainer">New number of containers.</param>
        /// <returns></returns>
        Task SetNumberOfNodes(int numContainer);

        /// <summary>
        /// Enumerate all items to the output stream in batches.
        /// </summary>
        /// <param name="batchSize">Size of one batch.</param>
        /// <returns></returns>
        Task<int> EnumerateToStream(int batchSize = int.MaxValue);
    }
}