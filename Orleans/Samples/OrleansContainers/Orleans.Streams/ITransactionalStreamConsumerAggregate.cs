using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Streams
{
    /// <summary>
    /// Consumes data from a set of streams.
    /// </summary>
    /// <typeparam name="TIn">Type of data.</typeparam>
    public interface ITransactionalStreamConsumerAggregate<TIn> : ITransactionalStreamTearDown
    {
        /// <summary>
        /// Set input source.
        /// </summary>
        /// <param name="streamIdentities">Information about streams to subscribe to.</param>
        /// <returns></returns>
        Task SetInput(IEnumerable<StreamIdentity<TIn>> streamIdentities);

        /// <summary>
        /// Wait for a transaction to be completed.
        /// </summary>
        /// <param name="transactionId">Transaction id to await.</param>
        /// <returns></returns>
        Task TransactionComplete(int transactionId);
    }
}