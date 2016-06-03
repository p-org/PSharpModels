using System.Collections.Generic;
using System.Threading.Tasks;

using Orleans;

namespace Raft
{
    /// <summary>
    /// Grain interface IServer.
    /// </summary>
	public interface IServer : IGrainWithIntegerKey
    {
        Task Configure(int id, List<int> serverIds, int clusterId);
        
        Task VoteRequest(int term, int candidateId, int lastLogIndex, int lastLogTerm);

        Task VoteResponse(int term, bool voteGranted);
    }
}
