using System.Threading.Tasks;
using Orleans;

namespace OrleansBuggyExample
{
    /// <summary>
    /// Grain interface ISender
    /// </summary>
	public interface ISender : IGrainWithIntegerKey
    {
        Task DoSomething(int numberOfItems);
    }
}
