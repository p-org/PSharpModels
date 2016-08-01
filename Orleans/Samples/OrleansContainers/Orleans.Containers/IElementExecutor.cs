using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Collections
{
    /// <summary>
    /// Execute operations on items hosted in containers.
    /// </summary>
    /// <typeparam name="T">Type of items.</typeparam>
    public interface IElementExecutor<T>
    {
        Task ExecuteSync(Action<T> action, ContainerElementReference<T> reference = null);
        Task ExecuteSync(Action<T, object> action, object state, ContainerElementReference<T> reference = null);

        Task<IList<object>> ExecuteSync(Func<T, object, object> func, object state);
        Task<IList<object>> ExecuteSync(Func<T, object> func);
        Task<object> ExecuteSync(Func<T, object, object> func, object state, ContainerElementReference<T> reference);
        Task<object> ExecuteSync(Func<T, object> func, ContainerElementReference<T> reference);

        Task ExecuteAsync(Func<T, Task> func, ContainerElementReference<T> reference = null);
        Task ExecuteAsync(Func<T, object, Task> func, object state, ContainerElementReference<T> reference = null);

        Task<IList<object>> ExecuteAsync(Func<T, Task<object>> func);
        Task<IList<object>> ExecuteAsync(Func<T, object, Task<object>> func, object state);
        Task<object> ExecuteAsync(Func<T, Task<object>> func, ContainerElementReference<T> reference);
        Task<object> ExecuteAsync(Func<T, object, Task<object>> func, object state, ContainerElementReference<T> reference);


        // TODO make generic callls possible with Orleans.
    }
}