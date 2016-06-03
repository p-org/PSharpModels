using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;

using Orleans;

namespace Raft
{
    /// <summary>
    /// Grain implementation class Server.
    /// </summary>
    public class Server : Grain<int>, IServer
    {
        #region fields

        /// <summary>
        /// The id of this server.
        /// </summary>
        private int ServerId;

        /// <summary>
        /// The server role.
        /// </summary>
        private Role Role;

        /// <summary>
        /// The cluster manager machine.
        /// </summary>
        private IClusterManager ClusterManager;

        /// <summary>
        /// The servers.
        /// </summary>
        private IDictionary<int, IServer> Servers;

        /// <summary>
        /// Leader.
        /// </summary>
        IServer Leader;

        /// <summary>
        /// The election timer of this server.
        /// </summary>
        IDisposable ElectionTimer;

        /// <summary>
        /// The periodic timer of this server.
        /// </summary>
        IDisposable PeriodicTimer;

        /// <summary>
        /// Latest term server has seen (initialized to 0 on
        /// first boot, increases monotonically).
        /// </summary>
        int CurrentTerm;

        /// <summary>
        /// Candidate that received vote in current term (or null if none).
        /// </summary>
        IServer VotedFor;

        /// <summary>
        /// Log entries.
        /// </summary>
        List<Log> Logs;

        /// <summary>
        /// Index of highest log entry known to be committed (initialized
        /// to 0, increases monotonically). 
        /// </summary>
        int CommitIndex;

        /// <summary>
        /// Index of highest log entry applied to state machine (initialized
        /// to 0, increases monotonically).
        /// </summary>
        int LastApplied;

        /// <summary>
        /// For each server, index of the next log entry to send to that
        /// server (initialized to leader last log index + 1). 
        /// </summary>
        Dictionary<IServer, int> NextIndex;

        /// <summary>
        /// For each server, index of highest log entry known to be replicated
        /// on server (initialized to 0, increases monotonically).
        /// </summary>
        Dictionary<IServer, int> MatchIndex;

        /// <summary>
        /// Number of received votes.
        /// </summary>
        int VotesReceived;

        /// <summary>
        /// The latest client request.
        /// </summary>
        //Client.Request LastClientRequest;

        #endregion

        #region methods

        public override Task OnActivateAsync()
        {
            if (this.CurrentTerm == 0)
            {
                this.Leader = null;
                this.VotedFor = null;

                this.Logs = new List<Log>();

                this.CommitIndex = 0;
                this.LastApplied = 0;

                this.Servers = new Dictionary<int, IServer>();
                this.NextIndex = new Dictionary<IServer, int>();
                this.MatchIndex = new Dictionary<IServer, int>();
            }
            
            return base.OnActivateAsync();
        }

        public Task Configure(int id, List<int> serverIds, int clusterId)
        {
            if (this.Servers.Count == 0)
            {
                this.ServerId = id;

                foreach (var idx in serverIds)
                {
                    this.Servers.Add(idx, GrainClient.GrainFactory.GetGrain<IServer>(idx));
                }

                this.ClusterManager = GrainClient.GrainFactory.GetGrain<IClusterManager>(clusterId);

                //this.ElectionTimer = this.CreateMachine(typeof(ElectionTimer));
                //this.Send(this.ElectionTimer, new ElectionTimer.ConfigureEvent(this.Id));

                //this.PeriodicTimer = this.CreateMachine(typeof(PeriodicTimer));
                //this.Send(this.PeriodicTimer, new PeriodicTimer.ConfigureEvent(this.Id));

                this.Role = Role.Follower;
            }
            
            return new Task(() => { });
        }

        #endregion
    }
}
