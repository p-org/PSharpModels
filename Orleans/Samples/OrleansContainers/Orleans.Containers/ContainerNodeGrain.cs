using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans.Collections.Utilities;
using Orleans.Streams;
using Orleans.Streams.Endpoints;
using Orleans.Streams.Messages;

namespace Orleans.Collections
{
    /// <summary>
    ///     Implementation of a containerNode grain.
    /// </summary>
    public class ContainerNodeGrain<T> : Grain, IContainerNodeGrain<T>
    {
        protected IList<T> Collection;
        protected SingleStreamProvider<ContainerHostedElement<T>> StreamProvider;
        private SingleStreamConsumer<T> _streamConsumer;
        private const string StreamProviderName = "CollectionStreamProvider";

        public async Task<IReadOnlyCollection<ContainerElementReference<T>>> AddRange(IEnumerable<T> items)
        {
            var newReferences = await InternalAddItems(items);

            return newReferences;
        }

        public async Task EnumerateItems(ICollection<IBatchItemAdder<T>> adders)
        {
            await adders.BatchAdd(Collection);
        }

        public Task Clear()
        {
            Collection.Clear();
            return TaskDone.Done;
        }

        public Task<bool> Contains(T item)
        {
            return Task.FromResult(Collection.Contains(item));
        }

        public Task<int> Count()
        {
            return Task.FromResult(Collection.Count);
        }


        public async Task<bool> Remove(T item)
        {
            return await InternalRemove(item);
        }

        protected virtual Task<bool> InternalRemove(T item)
        {
            return Task.FromResult(Collection.Remove(item));
        }

        public async Task<bool> Remove(ContainerElementReference<T> reference)
        {
            if (!reference.ContainerId.Equals(this.GetPrimaryKey()))
            {
                throw new ArgumentException();
            }

            return await InternalRemove(reference);
        }

        protected virtual Task<bool> InternalRemove(ContainerElementReference<T> reference)
        {
            if (Collection.Count < reference.Offset)
            {
                return Task.FromResult(false);
            }

            Collection.RemoveAt(reference.Offset);

            return Task.FromResult(true);
        }

        public async Task<int> EnumerateToStream(int? transactionId = null)
        {
            var hostedItems = Collection.Select((value, index) => new ContainerHostedElement<T>(GetReferenceForItem(index), value)).ToList();

            return await StreamProvider.SendItems(hostedItems, true, transactionId);
        }

        public override async Task OnActivateAsync()
        {
            Collection = new List<T>();
            StreamProvider = new SingleStreamProvider<ContainerHostedElement<T>>(GetStreamProvider(StreamProviderName), this.GetPrimaryKey());
            _streamConsumer = new SingleStreamConsumer<T>(GetStreamProvider(StreamProviderName), this, TearDown);
            await base.OnActivateAsync();
        }

        protected virtual Task<IReadOnlyCollection<ContainerElementReference<T>>> InternalAddItems(IEnumerable<T> batch)
        {
            lock (Collection)
            {
                var oldCount = Collection.Count;
                foreach (var item in batch)
                {
                    Collection.Add(item);
                }

                IReadOnlyCollection<ContainerElementReference<T>> newReferences =
                    Enumerable.Range(oldCount, Collection.Count - oldCount).Select(i => GetReferenceForItem(i)).ToList();
                return Task.FromResult(newReferences);
            }
        }

        protected T GetItemAt(int offset)
        {
            return Collection[offset];
        }

        protected ContainerElementReference<T> GetReferenceForItem(int offset, bool exists = true)
        {
            return new ContainerElementReference<T>(this.GetPrimaryKey(), offset, this,
                this.AsReference<IContainerNodeGrain<T>>(), exists);
        }

        public async Task<StreamIdentity<ContainerHostedElement<T>>> GetStreamIdentity()
        {
            return await StreamProvider.GetStreamIdentity();
        }

        public async Task SetInput(StreamIdentity<T> inputStream)
        {
            await _streamConsumer.SetInput(inputStream);
        }

        public Task TransactionComplete(int transactionId)
        {
            return _streamConsumer.TransactionComplete(transactionId);
        }

        public async Task TearDown()
        {
            await StreamProvider.TearDown();
        }

        public async Task<bool> IsTearedDown()
        {
            var tearDownStates = await Task.WhenAll(_streamConsumer.IsTearedDown(), StreamProvider.IsTearedDown());

            return tearDownStates[0] && tearDownStates[1];
        }

        public Task ExecuteSync(Action<T> action, ContainerElementReference<T> reference = null)
        {
            return ExecuteSync((x, state) => action(x), null, reference);
        }

        public Task ExecuteSync(Action<T, object> action, object state, ContainerElementReference<T> reference = null)
        {
            if (reference != null)
            {
                if (!reference.ContainerId.Equals(this.GetPrimaryKey()))
                {
                    throw new InvalidOperationException();
                }
                var curItem = GetItemAt(reference.Offset);
                action(curItem, state);
            }
            else
            {
                foreach (var item in Collection)
                {
                    action(item, state);
                }
            }

            return TaskDone.Done;
        }

        public Task<IList<object>> ExecuteSync(Func<T, object> func)
        {
            return ExecuteSync((x, state) => func(x), null);
        }

        public Task<object> ExecuteSync(Func<T, object, object> func, object state, ContainerElementReference<T> reference)
        {
            if (!this.GetPrimaryKey().Equals(reference.ContainerId))
            {
                throw new InvalidOperationException();
            }
            var curItem = GetItemAt(reference.Offset);
            var result = func(curItem, state);
            return Task.FromResult(result);
        }

        public Task<IList<object>> ExecuteSync(Func<T, object, object> func, object state)
        {
            IList<object> results = Collection.Select(item => func(item, state)).ToList();
            return Task.FromResult(results);
        }

        public Task<object> ExecuteSync(Func<T, object> func, ContainerElementReference<T> reference = null)
        {
            return ExecuteSync((x, state) => func(x), null, reference);
        }

        public Task ExecuteAsync(Func<T, Task> func, ContainerElementReference<T> reference = null)
        {
            return ExecuteAsync((x, state) => func(x), null, reference);
        }

        public async Task ExecuteAsync(Func<T, object, Task> func, object state, ContainerElementReference<T> reference = null)
        {
            if (reference != null)
            {
                if (!this.GetPrimaryKey().Equals(reference.ContainerId))
                {
                    throw new InvalidOperationException();
                }
                var curItem = GetItemAt(reference.Offset);
                await func(curItem, state);
            }

            else
            {
                foreach (var item in Collection)
                {
                    await func(item, state);
                }
            }
        }

        public Task<IList<object>> ExecuteAsync(Func<T, Task<object>> func)
        {
            return ExecuteAsync((x, state) => func(x), null);
        }

        public async Task<IList<object>> ExecuteAsync(Func<T, object, Task<object>> func, object state)
        {
            var results = Collection.Select(item => func(item, state)).ToList();
            var resultSet = await Task.WhenAll(results);
            return new List<object>(resultSet);
        }

        public Task<object> ExecuteAsync(Func<T, Task<object>> func, ContainerElementReference<T> reference)
        {
            return ExecuteAsync((x, state) => func(x), null, reference);
        }

        public async Task<object> ExecuteAsync(Func<T, object, Task<object>> func, object state, ContainerElementReference<T> reference)
        {
            if (!this.GetPrimaryKey().Equals(reference.ContainerId))
            {
                throw new InvalidOperationException();
            }
            var curItem = GetItemAt(reference.Offset);
            var result = await func(curItem, state);
            return result;
        }

        public async Task Visit(ItemMessage<T> message)
        {
            await InternalAddItems(message.Items);
        }

        public Task Visit(TransactionMessage transactionMessage)
        {
            return TaskDone.Done;
        }
    }
}