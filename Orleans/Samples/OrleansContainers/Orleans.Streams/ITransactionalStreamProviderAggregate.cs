using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Streams
{
    /// <summary>
    /// Provides multiple streams of data.
    /// </summary>
    /// <typeparam name="TOut">Type of data.</typeparam>
    public interface ITransactionalStreamProviderAggregate<TOut> : ITransactionalStreamTearDown
    {
        /// <summary>
        /// Get identities of the provided streams.
        /// </summary>
        /// <returns></returns>
        Task<IList<StreamIdentity<TOut>>> GetStreamIdentities();
    }
}