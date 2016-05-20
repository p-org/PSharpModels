using System.Threading.Tasks;
using Orleans;

namespace BasicOrleansApp
{
    /// <summary>
    /// Grain interface IServer
    /// </summary>
	public interface IServer : IGrainWithIntegerKey
    {
        Task<int> Ping(IClient client, int counter);
    }
}
