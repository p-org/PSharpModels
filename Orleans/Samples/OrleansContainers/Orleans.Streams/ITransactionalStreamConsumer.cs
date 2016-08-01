using System;
using System.Threading.Tasks;
using Orleans.Streams.Messages;

namespace Orleans.Streams
{
    public interface ITransactionalStreamConsumer<TIn> : ITransactionalStreamTearDown, IStreamMessageVisitor<TIn>
    {
        Task SetInput(StreamIdentity<TIn> inputStream);

        Task TransactionComplete(int transactionId);
    }
}