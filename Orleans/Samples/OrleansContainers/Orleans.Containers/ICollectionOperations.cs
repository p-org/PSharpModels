using System.Threading.Tasks;

namespace Orleans.Collections
{
    public interface ICollectionOperations<T> : IBatchWriter<T>
    {
        /// <summary>
        /// Clears all items from the container.
        /// </summary>
        /// <returns></returns>
        Task Clear();


        /// <summary>
        /// Get number of items.
        /// </summary>
        /// <returns></returns>
        Task<int> Count();

        /// <summary>
        /// Determines if the collection contains a specific value.
        /// </summary>
        /// <param name="item">Value to check for.</param>
        /// <returns></returns>
        Task<bool> Contains(T item);

        /// <summary>
        /// Remove first equal item from collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if at least one reference has been removed.</returns>
        Task<bool> Remove(T item);

        /// <summary>
        /// Removes reference from the collection.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        Task<bool> Remove(ContainerElementReference<T> reference);
    }
}