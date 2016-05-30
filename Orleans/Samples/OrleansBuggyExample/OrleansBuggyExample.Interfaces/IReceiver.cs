using System.Threading.Tasks;
using Orleans;

namespace OrleansBuggyExample
{
    /// <summary>
    /// Grain interface IReceiver
    /// </summary>
	public interface IReceiver : IGrainWithIntegerKey
    {
        Task StartTransaction();

        Task TransmitData(string item);

        Task<int> GetCurrentCount();
    }
}
