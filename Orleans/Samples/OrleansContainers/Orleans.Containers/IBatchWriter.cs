using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Collections
{
    /// <summary>
    /// The type implementing this interface can write items of type T to consumers.
    /// </summary>
    /// <typeparam name="T">Type of the objects that are written.</typeparam>
    public interface IBatchWriter<T>
    {
        Task EnumerateItems(ICollection<IBatchItemAdder<T>> adders);

    }
}