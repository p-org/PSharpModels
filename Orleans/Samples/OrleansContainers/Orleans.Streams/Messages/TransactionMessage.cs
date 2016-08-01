using System;
using System.Threading.Tasks;

namespace Orleans.Streams.Messages
{
    /// <summary>
    /// Holds information about a transaction within a stream.
    /// </summary>
    [Serializable]
    public struct TransactionMessage : IEquatable<TransactionMessage>, IStreamMessage
    {
        public TransactionState State;
        public int TransactionId;

        public bool Equals(TransactionMessage other)
        {
            return other.State.Equals(this.State) && other.TransactionId == this.TransactionId;
        }

        public async Task Accept(IStreamMessageVisitor visitor)
        {
            await visitor.Visit(this);
        }
    }

    /// <summary>
    /// State of the transaction.
    /// </summary>
    public enum TransactionState
    {
        Start, End
    }
}