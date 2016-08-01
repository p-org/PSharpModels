using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Collections
{
    /// <summary>
    /// Gives access to consumers that can receive an item of type T.
    /// </summary>
    /// <typeparam name="T">Type of objects to write.</typeparam>
    public interface IBatchWriteable<T>
    {
        /// <summary>
        /// Get consumers that accept an item of type T.
        /// </summary>
        /// <returns></returns>
        Task<ICollection<IBatchItemAdder<T>>> GetItemAdders();
    }
}