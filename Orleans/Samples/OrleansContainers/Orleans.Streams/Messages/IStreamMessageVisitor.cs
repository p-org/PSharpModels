using System.Threading.Tasks;

namespace Orleans.Streams.Messages
{
    public interface IStreamMessageVisitor<T> : IStreamMessageVisitor
    {
        Task Visit(ItemMessage<T> message);
    }

    public interface IStreamMessageVisitor
    {
        Task Visit(TransactionMessage transactionMessage);
    }
}