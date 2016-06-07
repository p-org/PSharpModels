using System.Threading.Tasks;

using Microsoft.ServiceFabric.Actors;

namespace FailureDetector.Interfaces
{
    public interface ISafetyMonitor : IActor
    {
        Task NotifyPing(int nodeId);

        Task NotifyPong(int nodeId);
    }
}
