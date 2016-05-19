using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActorModel;
using Microsoft.ServiceFabric.Data;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class ActorStateManager : IActorStateManager
    {
        private Dictionary<string, object> store;

        public ActorStateManager()
        {
            store = new Dictionary<string, object>();
        }

        Task<T> IActorStateManager.AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken)
        {
            if (store.ContainsKey(stateName))
            {
                object updatedValue = new object();
                updatedValue = updateValueFactory(stateName, (T)store[stateName]);
                store[stateName] = updatedValue;
            }
            else
            {
                store.Add(stateName, addValue);
            }

            return DummyTask.FromResult(addValue);
        }

        Task IActorStateManager.AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            store.Add(stateName, value);
            return DummyTask.FromResult(true);
        }

        Task<bool> IActorStateManager.ContainsStateAsync(string stateName, CancellationToken cancellationToken)
        {
            return DummyTask.FromResult(store.ContainsKey(stateName));
        }

        Task<T> IActorStateManager.GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            if (store.ContainsKey(stateName))
            {
                return DummyTask.FromResult((T)store[stateName]);
            }
            else
            {
                store.Add(stateName, value);
                return DummyTask.FromResult(value);
            }

        }

        Task<T> IActorStateManager.GetStateAsync<T>(string stateName, CancellationToken cancellationToken)
        {
            return DummyTask.FromResult((T)store[stateName]);
        }

        Task<IEnumerable<string>> IActorStateManager.GetStateNamesAsync(CancellationToken cancellationToken)
        {
            return DummyTask.FromResult((IEnumerable<string>)store.Keys);
        }

        Task IActorStateManager.RemoveStateAsync(string stateName, CancellationToken cancellationToken)
        {
            store.Remove(stateName);
            return DummyTask.FromResult(true);
        }

        Task IActorStateManager.SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            if (!store.ContainsKey(stateName))
                store.Add(stateName, value);
            else
                store[stateName] = value;
            return DummyTask.FromResult(true);
        }

        Task<bool> IActorStateManager.TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            store.Add(stateName, value);
            return DummyTask.FromResult(true);
        }

        Task<ConditionalValue<T>> IActorStateManager.TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken)
        {
            if (store.ContainsKey(stateName))
            {
                return DummyTask.FromResult(new ConditionalValue<T>(true, (T)store[stateName]));
            }
            else
            {
                return DummyTask.FromResult(new ConditionalValue<T>(false));
            }
        }

        Task<bool> IActorStateManager.TryRemoveStateAsync(string stateName, CancellationToken cancellationToken)
        {
            if (store.ContainsKey(stateName))
            {
                store.Remove(stateName);
                return DummyTask.FromResult(true);
            }
            else
                return DummyTask.FromResult(false);
        }
    }
}
