using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Actors;

namespace FailureDetector.Interfaces
{
    public interface IFailureDetector : IActor
    {
        Task Configure(List<int> nodeIds);

        Task Start();

        Task Pong(ulong requestId, int senderId);

        Task RegisterClient(int clientId);

        Task UnregisterClient(int clientId);
    }
}
