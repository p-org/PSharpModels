using System.Threading.Tasks;

namespace Orleans.Streams.Messages
{
    public interface IStreamMessage<T> : IStreamMessage
    {
    }

    public interface IStreamMessage
    {
        Task Accept(IStreamMessageVisitor visitor);
    }

}