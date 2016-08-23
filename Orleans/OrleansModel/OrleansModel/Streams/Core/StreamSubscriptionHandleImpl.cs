using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Streams.Core
{
    public class StreamSubscriptionHandleImpl<T> : StreamSubscriptionHandle<T>
    {
        private StreamIdentity StreamId;

        public StreamSubscriptionHandleImpl(StreamIdentity streamId)
        {
            this.StreamId = streamId;
        }
        public override Guid HandleId
        {
            get
            {
                return StreamId.Guid;
            }
        }

        public override IStreamIdentity StreamIdentity
        {
            get
            {
                return StreamId;
            }
        }

        public override bool Equals(StreamSubscriptionHandle<T> other)
        {
            if (StreamId.Equals(other.StreamIdentity))
                return true;
            else
                return false;
        }

        public override Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncObserver<T> observer, StreamSequenceToken token = null)
        {
            throw new NotImplementedException();
        }

        public override Task UnsubscribeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
