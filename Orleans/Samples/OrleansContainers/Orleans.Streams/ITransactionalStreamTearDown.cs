using System.Threading.Tasks;

namespace Orleans.Streams
{
    /// <summary>
    /// Supports a tear down operation to dispose resources.
    /// </summary>
    public interface ITransactionalStreamTearDown
    {
        /// <summary>
        /// Unsubscribes from all streams this entity consumes and remove references to stream this entity produces.
        /// </summary>
        /// <returns></returns>
        Task TearDown();

        /// <summary>
        /// Checks if this entity is teared down.
        /// </summary>
        /// <returns></returns>
        Task<bool> IsTearedDown();
    }
}