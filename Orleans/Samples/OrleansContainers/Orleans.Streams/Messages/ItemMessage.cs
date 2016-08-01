using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Streams.Messages
{
    [Serializable]
    public class ItemMessage<T> : IStreamMessage<T>
    {
        public IEnumerable<T> Items { get; private set; }

        public ItemMessage(IEnumerable<T> items)
        {
            Items = items;
        }

        public async Task Accept(IStreamMessageVisitor<T> visitor)
        {
            await visitor.Visit(this);
        }

        public async Task Accept(IStreamMessageVisitor visitor)
        {
            var messageVisitor = visitor as IStreamMessageVisitor<T>;
            if (messageVisitor != null)
            {
                await messageVisitor.Visit(this);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}