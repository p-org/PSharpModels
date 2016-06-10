using System.Threading.Tasks;
using Orleans;

namespace Raft
{
    /// <summary>
    /// Grain interface IClusterManager.
    /// </summary>
	public interface IClusterManager : IGrainWithIntegerKey
    {
        Task Configure();

        Task NotifyLeaderUpdate(int leaderId, int term);

        Task RelayClientRequest(int clientId, int command);

        Task RedirectClientRequest(int clientId, int command);
    }
}
