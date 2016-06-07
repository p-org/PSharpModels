using System.Threading.Tasks;

using Microsoft.ServiceFabric.Actors;

namespace FailureDetector
{
    public interface INode : IActor
    {
        Task Configure(int id);

        Task Ping(ulong requestId, int senderId);
    }
}
