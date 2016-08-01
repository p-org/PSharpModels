using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Collections
{
    public interface IBatchItemAdder<T>
    {
        Task<IReadOnlyCollection<ContainerElementReference<T>>> AddRange(IEnumerable<T> items);
    }
}