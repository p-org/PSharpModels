﻿//-----------------------------------------------------------------------
// <copyright file="ActorStateManager.cs">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Data;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class ActorStateManager : IActorStateManager
    {
        private ConcurrentDictionary<string, object> store;

        public ActorStateManager()
        {
            store = new ConcurrentDictionary<string, object>();
        }

        System.Threading.Tasks.Task<T> IActorStateManager.AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken)
        {
            if (store.ContainsKey(stateName))
            {
                object updatedValue = new object();
                updatedValue = updateValueFactory(stateName, (T)store[stateName]);
                store[stateName] = updatedValue;
            }
            else
            {
                store.TryAdd(stateName, addValue);
            }

            return Task.FromResult(addValue);
        }

        Task IActorStateManager.AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            store.TryAdd(stateName, value);
            return Task.FromResult(true);
        }

        System.Threading.Tasks.Task<bool> IActorStateManager.ContainsStateAsync(string stateName, CancellationToken cancellationToken)
        {
            return Task.FromResult(store.ContainsKey(stateName));
        }

        System.Threading.Tasks.Task<T> IActorStateManager.GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            if (store.ContainsKey(stateName))
            {
                return Task.FromResult((T)store[stateName]);
            }
            else
            {
                store.TryAdd(stateName, value);
                return Task.FromResult(value);
            }

        }

        System.Threading.Tasks.Task<T> IActorStateManager.GetStateAsync<T>(string stateName, CancellationToken cancellationToken)
        {
            return Task.FromResult((T)store[stateName]);
        }

        System.Threading.Tasks.Task<IEnumerable<string>> IActorStateManager.GetStateNamesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IEnumerable<string>)store.Keys);
        }

        Task IActorStateManager.RemoveStateAsync(string stateName, CancellationToken cancellationToken)
        {
            object value;
            store.TryRemove(stateName, out value);
            return Task.FromResult(true);
        }

        Task IActorStateManager.SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            if (!store.ContainsKey(stateName))
                store.TryAdd(stateName, value);
            else
                store[stateName] = value;
            return Task.FromResult(true);
        }

        System.Threading.Tasks.Task<bool> IActorStateManager.TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken)
        {
            store.TryAdd(stateName, value);
            return Task.FromResult(true);
        }

        System.Threading.Tasks.Task<ConditionalValue<T>> IActorStateManager.TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken)
        {
            if (store.ContainsKey(stateName))
            {
                return Task.FromResult(new ConditionalValue<T>(true, (T)store[stateName]));
            }
            else
            {
                return Task.FromResult(new ConditionalValue<T>(false));
            }
        }

        System.Threading.Tasks.Task<bool> IActorStateManager.TryRemoveStateAsync(string stateName, CancellationToken cancellationToken)
        {
            if (store.ContainsKey(stateName))
            {
                object value;
                store.TryRemove(stateName, out value);
                return Task.FromResult(true);
            }
            else
                return Task.FromResult(false);
        }
    }
}
