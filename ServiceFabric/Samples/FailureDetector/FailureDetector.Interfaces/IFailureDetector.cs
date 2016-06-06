using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Actors;

namespace FailureDetector
{
    public interface IFailureDetector : IActor
    {
        Task Configure(List<int> nodeIds);

        Task RegisterClient(int clientId);
    }
}
