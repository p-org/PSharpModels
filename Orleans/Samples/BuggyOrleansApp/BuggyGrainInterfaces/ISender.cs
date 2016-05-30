using System.Threading.Tasks;
using Orleans;

namespace BuggyOrleansApp
{
    /// <summary>
    /// Grain interface IServer
    /// </summary>
	public interface ISender : IGrainWithIntegerKey
    {
        Task DoSomething(int numberOfItems);
    }
}
