using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Orleans.Collections
{
    public class ContainerGrain<T> : Grain, IGrainWithIntegerKey, IContainerGrain<T>
    {
        private const int NumberContainersStart = 1;
        private List<IContainerNodeGrain<T>> _containers;
        private int _lastTransactionId;
        private bool _tearDownExecuted;

        public Task<ICollection<IBatchItemAdder<T>>> GetItemAdders()
        {
            ICollection<IBatchItemAdder<T>> readers = _containers.Cast<IBatchItemAdder<T>>().ToList();

            return Task.FromResult(readers);
        }

        public async Task EnumerateItems(ICollection<IBatchItemAdder<T>> consumers)
        {
            if (consumers.Count == 0)
            {
                return;
            }

            var tasks = _containers.Select(c => c.EnumerateItems(consumers)).ToList();
            await Task.WhenAll(tasks);
        }

        public async Task Clear()
        {
            await Task.WhenAll(_containers.Select(async x => await x.Clear()).ToList());
            _containers.Clear();
        }

        public async Task<bool> Contains(T item)
        {
            var resultTask = Task.WhenAll(_containers.Select(async c => await c.Contains(item)));
            var results = await resultTask;

            return results.Contains(true);
        }

        public async Task<int> Count()
        {
            var resultTask = await Task.WhenAll(_containers.Select(async container => await container.Count()));
            return resultTask.Sum();
        }

        public async Task<bool> Remove(T item)
        {
            var removed = false;
            foreach (var container in _containers)
            {
                removed = await container.Remove(item);
                if (removed)
                    break;
            }

            return removed;
        }

        public async Task<bool> Remove(ContainerElementReference<T> reference)
        {
            var container = _containers.First(c => c.GetPrimaryKey().Equals(reference.ContainerId));
            if (container != null)
            {
                return await container.Remove(reference);
            }

            return false;
        }

        public async Task<int> EnumerateToStream(int batchSize)
        {
            var transactionId = ++_lastTransactionId;
            await Task.WhenAll(_containers.Select(c => c.EnumerateToStream(transactionId)));

            return transactionId;
        }

        public Task<IList<object>> ExecuteAsync(Func<T, Task<object>> func)
        {
            throw new NotImplementedException();
        }

        public Task<object> ExecuteAsync(Func<T, Task<object>> func, ContainerElementReference<T> reference)
        {
            throw new NotImplementedException();
        }

        public Task<IList<object>> ExecuteAsync(Func<T, object, Task<object>> func, object state)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(Func<T, Task> func, ContainerElementReference<T> reference = null)
        {
            throw new NotImplementedException();
        }

        public Task<object> ExecuteAsync(Func<T, object, Task<object>> func, object state, ContainerElementReference<T> reference)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(Func<T, object, Task> func, object state, ContainerElementReference<T> reference = null)
        {
            throw new NotImplementedException();
        }

        public Task<IList<object>> ExecuteSync(Func<T, object> func)
        {
            throw new NotImplementedException();
        }

        public Task<object> ExecuteSync(Func<T, object> func, ContainerElementReference<T> reference)
        {
            throw new NotImplementedException();
        }

        public Task<IList<object>> ExecuteSync(Func<T, object, object> func, object state)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteSync(Action<T> action, ContainerElementReference<T> reference = null)
        {
            throw new NotImplementedException();
        }

        public Task<object> ExecuteSync(Func<T, object, object> func, object state, ContainerElementReference<T> reference)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteSync(Action<T, object> action, object state, ContainerElementReference<T> reference = null)
        {
            throw new NotImplementedException();
        }

       
        public Task<IList<StreamIdentity<ContainerHostedElement<T>>>> GetStreamIdentities()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTearedDown()
        {
            throw new NotImplementedException();
        }

        
        public Task SetInput(IEnumerable<StreamIdentity<T>> streamIdentities)
        {
            throw new NotImplementedException();
        }

        public async Task SetNumberOfNodes(int numContainer)
        {
            var containersToAdd = numContainer - _containers.Count;
            if (containersToAdd < 0)
            {
                throw new NotImplementedException("Merging containers is not implemented yet.");
            }


            //var initTasks = new List<Task>();
            for (var i = 0; i < containersToAdd; i++)
            {
                var containerNode = CreateContainerGrain();
                ////initTasks.Add(await containerNode.Clear());
                _containers.Add(containerNode);
            }

            //await Task.WhenAll(initTasks);
            await Task.FromResult(true);

        }

        ////Remove this
        //public IContainerGrain_PSharpProxy(object primaryKey)
        //{
        //    object target = new ContainerGrain<T>();
        //}

        public Task TearDown()
        {
            throw new NotImplementedException();
        }

        public Task TransactionComplete(int transactionId)
        {
            throw new NotImplementedException();
        }

        //public async Task ExecuteAsync(Func<T, Task> func, ContainerElementReference<T> reference = null)
        //{
        //    if (reference != null)
        //    {
        //        var container = _containers.First(c => c.GetPrimaryKey().Equals(reference.ContainerId));
        //        await container.ExecuteAsync(func, reference);
        //    }
        //    else
        //    {
        //        await Task.WhenAll(_containers.Select(c => c.ExecuteAsync(func)));
        //    }
        //}

        //public async Task ExecuteAsync(Func<T, object, Task> func, object state, ContainerElementReference<T> reference = null)
        //{
        //    if (reference != null)
        //    {
        //        var container = _containers.First(c => c.GetPrimaryKey().Equals(reference.ContainerId));
        //        await container.ExecuteAsync(func, state, reference);
        //    }
        //    else
        //    {
        //        await Task.WhenAll(_containers.Select(c => c.ExecuteAsync(func, state)));
        //    }
        //}

        //public async Task<IList<object>> ExecuteAsync(Func<T, Task<object>> func)
        //{
        //    var result = await Task.WhenAll(_containers.Select(c => c.ExecuteAsync(func)));
        //    return new List<object>(result);
        //}

        //public async Task<IList<object>> ExecuteAsync(Func<T, object, Task<object>> func, object state)
        //{
        //    var result = await Task.WhenAll(_containers.Select(c => c.ExecuteAsync(func, state)));
        //    return new List<object>(result);
        //}

        //public async Task<object> ExecuteAsync(Func<T, Task<object>> func, ContainerElementReference<T> reference)
        //{
        //    var container = _containers.First(c => c.GetPrimaryKey().Equals(reference.ContainerId));
        //    return await container.ExecuteAsync(func, reference);
        //}

        //public async Task<object> ExecuteAsync(Func<T, object, Task<object>> func, object state, ContainerElementReference<T> reference)
        //{
        //    var container = _containers.First(c => c.GetPrimaryKey().Equals(reference.ContainerId));
        //    return await container.ExecuteAsync(func, state, reference);
        //}

        //public async Task ExecuteSync(Action<T> action, ContainerElementReference<T> reference = null)
        //{
        //    if (reference != null)
        //    {
        //        var container = _containers.First(c => c.GetPrimaryKey().Equals(reference.ContainerId));
        //        await container.ExecuteSync(action, reference);
        //    }
        //    else
        //    {
        //        await Task.WhenAll(_containers.Select(c => c.ExecuteSync(action, null)));
        //    }
        //}

        //public async Task ExecuteSync(Action<T, object> action, object state, ContainerElementReference<T> reference = null)
        //{
        //    if (reference != null)
        //    {
        //        var container = _containers.First(c => c.GetPrimaryKey().Equals(reference.ContainerId));
        //        await container.ExecuteSync(action, state, reference);
        //    }
        //    else
        //    {
        //        await Task.WhenAll(_containers.Select(c => c.ExecuteSync(action, state)));
        //    }
        //}

        //public async Task<IList<object>> ExecuteSync(Func<T, object, object> func, object state)
        //{
        //    var result = Task.WhenAll(_containers.Select(c => c.ExecuteSync(func, state)));
        //    return await result;
        //}

        //public async Task<IList<object>> ExecuteSync(Func<T, object> func)
        //{
        //    var result = Task.WhenAll(_containers.Select(c => c.ExecuteSync(func)));
        //    return await result;
        //}

        //public async Task<object> ExecuteSync(Func<T, object, object> func, object state, ContainerElementReference<T> reference)
        //{
        //    var container = _containers.First(c => c.GetPrimaryKey().Equals(reference.ContainerId));
        //    return await container.ExecuteSync(func, state, reference);
        //}

        //public async Task<object> ExecuteSync(Func<T, object> func, ContainerElementReference<T> reference)
        //{
        //    var container = _containers.First(c => c.GetPrimaryKey().Equals(reference.ContainerId));
        //    return await container.ExecuteSync(func, reference);
        //}

        //public async Task SetInput(IEnumerable<StreamIdentity<T>> streamIdentities)
        //{
        //    _tearDownExecuted = false;
        //    if (streamIdentities.Count() != _containers.Count)
        //    {
        //        throw new ArgumentException();
        //    }

        //    await
        //        Task.WhenAll(_containers.Zip(streamIdentities,
        //            (grain, identity) => new Tuple<IContainerNodeGrain<T>, StreamIdentity<T>>(grain, identity))
        //            .Select(t => t.Item1.SetInput(t.Item2)));
        //}

        //public async Task TransactionComplete(int transactionId)
        //{
        //    await Task.WhenAll(_containers.Select(c => c.TransactionComplete(transactionId)));
        //}

        //public async Task<IList<StreamIdentity<ContainerHostedElement<T>>>> GetStreamIdentities()
        //{
        //    var streamTasks = await Task.WhenAll(_containers.Select(c => c.GetStreamIdentity()));

        //    return new List<StreamIdentity<ContainerHostedElement<T>>>(streamTasks);
        //}

        //public Task<bool> IsTearedDown()
        //{
        //    return Task.FromResult(_tearDownExecuted);
        //}

        //public async Task TearDown()
        //{
        //    await Task.WhenAll(_containers.Select(c => c.TearDown()));
        //}

        //public override async Task OnActivateAsync()
        //{
        //    _containers = new List<IContainerNodeGrain<T>>();
        //    _lastTransactionId = -1;
        //    await SetNumberOfNodes(NumberContainersStart);
        //    await base.OnActivateAsync();
        //}

        internal virtual IContainerNodeGrain<T> CreateContainerGrain()
        {
            return GrainFactory.GetGrain<IContainerNodeGrain<T>>(Guid.NewGuid());
        }
    }
}