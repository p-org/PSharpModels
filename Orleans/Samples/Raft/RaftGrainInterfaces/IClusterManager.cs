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
    }
}
