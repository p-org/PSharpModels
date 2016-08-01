using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Streams.Endpoints
{
    /// <summary>
    /// Consumes items from multiple streams and places them in a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiStreamListConsumer<T> : MultiStreamConsumer<T>
    {
        public List<T> Items { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="streamProvider">Stream provider to use.</param>
        public MultiStreamListConsumer(IStreamProvider streamProvider) : base(streamProvider)
        {
            Items = new List<T>();
            StreamItemBatchReceivedFunc = enumerable =>
            {
                Items.AddRange(enumerable);
                return TaskDone.Done;
            };
        }
    }
}