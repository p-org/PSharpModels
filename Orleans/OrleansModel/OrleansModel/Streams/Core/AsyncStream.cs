using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Streams.Core
{
    internal class AsyncStream<T> : IAsyncStream<T>
    {
        public Guid Guid
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsRewindable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Namespace
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ProviderName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int CompareTo(IAsyncStream<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IAsyncStream<T> other)
        {
            throw new NotImplementedException();
        }

        public Task<IList<StreamSubscriptionHandle<T>>> GetAllSubscriptionHandles()
        {
            throw new NotImplementedException();
        }

        public Task OnCompletedAsync()
        {
            throw new NotImplementedException();
        }

        public Task OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task OnNextAsync(T item, StreamSequenceToken token = null)
        {
            throw new NotImplementedException();
        }

        public Task OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken token = null)
        {
            throw new NotImplementedException();
        }

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer)
        {
            throw new NotImplementedException();
        }
    }
}
