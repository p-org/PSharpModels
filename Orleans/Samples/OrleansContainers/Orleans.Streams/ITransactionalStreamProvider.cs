using System.Threading.Tasks;

namespace Orleans.Streams
{
    /// <summary>
    /// Provides a stream of data.
    /// </summary>
    /// <typeparam name="TOut">Type of data.</typeparam>
    public interface ITransactionalStreamProvider<TOut> : ITransactionalStreamTearDown
    {
        /// <summary>
        /// Get identity of the provided stream.
        /// </summary>
        /// <returns></returns>
        Task<StreamIdentity<TOut>> GetStreamIdentity();
    }
}