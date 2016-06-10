using System.Threading.Tasks;
using Orleans;

namespace Raft
{
    /// <summary>
    /// Grain interface IClient.
    /// </summary>
	public interface IClient : IGrainWithIntegerKey
    {
        Task Configure(int clusterId);

        Task ProcessResponse();
    }
}
