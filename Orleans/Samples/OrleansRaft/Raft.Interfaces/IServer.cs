using System.Collections.Generic;
using System.Threading.Tasks;

using Orleans;

namespace Raft.Interfaces
{
    /// <summary>
    /// Grain interface IServer.
    /// </summary>
	public interface IServer : IGrainWithIntegerKey
    {
        Task Configure(int id, List<int> serverIds, int clusterId);
        
        Task VoteRequest(int term, int candidateId, int lastLogIndex, int lastLogTerm);

        Task VoteResponse(int term, bool voteGranted);

        Task AppendEntriesRequest(int term, int leaderId, int prevLogIndex, int prevLogTerm,
            List<Log> entries, int leaderCommit, int clientId);

        Task AppendEntriesResponse(int term, bool success, int serverId, int clientId);

        Task RedirectClientRequest(int clientId, int command);

        Task ProcessClientRequest(int clientId, int command);
    }
}
