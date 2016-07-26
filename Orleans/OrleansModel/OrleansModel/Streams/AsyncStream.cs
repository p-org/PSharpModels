//-----------------------------------------------------------------------
// <copyright file="AsyncStream.cs">
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Streams
{
    internal class AsyncStream<T> : IAsyncStream<T>
    {
        public Guid Guid { get; private set; }

        public bool IsRewindable { get; private set; }

        public string Namespace { get; private set; }

        string IAsyncStream<T>.ProviderName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int IComparable<IAsyncStream<T>>.CompareTo(IAsyncStream<T> other)
        {
            throw new NotImplementedException();
        }

        bool IEquatable<IAsyncStream<T>>.Equals(IAsyncStream<T> other)
        {
            throw new NotImplementedException();
        }

        Task<IList<StreamSubscriptionHandle<T>>> IAsyncStream<T>.GetAllSubscriptionHandles()
        {
            throw new NotImplementedException();
        }

        Task IAsyncObserver<T>.OnCompletedAsync()
        {
            throw new NotImplementedException();
        }

        Task IAsyncObserver<T>.OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        Task IAsyncObserver<T>.OnNextAsync(T item, StreamSequenceToken token)
        {
            throw new NotImplementedException();
        }

        Task IAsyncBatchObserver<T>.OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken token)
        {
            throw new NotImplementedException();
        }

        Task<StreamSubscriptionHandle<T>> IAsyncObservable<T>.SubscribeAsync(IAsyncObserver<T> observer)
        {
            throw new NotImplementedException();
        }

        Task<StreamSubscriptionHandle<T>> IAsyncObservable<T>.SubscribeAsync(IAsyncObserver<T> observer, StreamSequenceToken token, StreamFilterPredicate filterFunc, object filterData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="streamId">Guid</param>
        /// <param name="streamNamespace">Namespace</param>
        /// <param name="isRewindable">IsRewindable</param>
        public AsyncStream(Guid streamId, string streamNamespace,
            bool isRewindable)
        {
            this.Guid = streamId;
            this.Namespace = streamNamespace;
            this.IsRewindable = isRewindable;
        }
    }
}