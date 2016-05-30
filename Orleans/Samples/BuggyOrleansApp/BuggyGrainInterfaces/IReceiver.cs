using System.Threading.Tasks;
using Orleans;

namespace BuggyOrleansApp
{
    /// <summary>
    /// Grain interface IClient
    /// </summary>
	public interface IReceiver : IGrainWithIntegerKey
    {
        Task StartTransaction();

        Task TransmitData(TransactionItems item);

        Task<int> GetCurrentCount();
    }
}
