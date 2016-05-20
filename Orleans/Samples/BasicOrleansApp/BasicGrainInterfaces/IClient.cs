using System.Threading.Tasks;
using Orleans;

namespace BasicOrleansApp
{
    /// <summary>
    /// Grain interface IClient
    /// </summary>
	public interface IClient : IGrainWithIntegerKey
    {
        Task<string> Initialize(IServer server);

        Task<int> Ping();

        Task<int> Pong(int counter);
    }
}
