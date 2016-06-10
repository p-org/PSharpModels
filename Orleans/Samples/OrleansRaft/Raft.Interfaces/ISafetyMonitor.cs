using System.Threading.Tasks;
using Orleans;

namespace Raft.Interfaces
{
    /// <summary>
    /// Grain interface ISafetyMonitor.
    /// </summary>
	public interface ISafetyMonitor : IGrainWithIntegerKey
    {
        Task NotifyLeaderElected(int term);
    }
}
