using System.Threading.Tasks;

using Microsoft.ServiceFabric.Actors;

namespace FailureDetector.Interfaces
{
    public interface INode : IActor
    {
        Task Configure(int id);

        Task Ping(ulong requestId, int senderId);

        Task Halt();
    }
}
