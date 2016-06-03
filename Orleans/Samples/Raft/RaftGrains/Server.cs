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
    [Reentrant]
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
        IServer VotedForCandidate;

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
                this.VotedForCandidate = null;

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
                
                this.BecomeFollower();
            }
            
            return new Task(() => { });
        }

        private void BecomeFollower()
        {
            ActorModel.Log($"<RaftLog> Server {this.ServerId} became FOLLOWER.");
            this.Role = Role.Follower;

            this.Leader = null;
            this.VotesReceived = 0;

            if (this.ElectionTimer != null)
            {
                this.ElectionTimer.Dispose();
            }

            if (this.PeriodicTimer != null)
            {
                this.PeriodicTimer.Dispose();
            }

            if (ActorModel.Random())
            {
                this.ElectionTimer = this.RegisterTimer(StartLeaderElection, null,
                    TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            }
        }

        private void BecomCandidate()
        {
            ActorModel.Log($"<RaftLog> Server {this.ServerId} became CANDIDATE.");
            this.Role = Role.Candidate;

            this.CurrentTerm++;
            this.VotedForCandidate = GrainClient.GrainFactory.GetGrain<IServer>(this.ServerId);
            this.VotesReceived = 1;

            ActorModel.Log($"<RaftLog> Candidate {this.ServerId} | term {this.CurrentTerm} " +
                $"| election votes {this.VotesReceived} | log {this.Logs.Count}.");

            this.BroadcastVoteRequests();
        }

        private void BecomeLeader()
        {
            ActorModel.Log($"<RaftLog> Leader {this.ServerId} | term {this.CurrentTerm} " +
                $"| election votes {this.VotesReceived} | log {this.Logs.Count}.");

            this.VotesReceived = 0;

            ActorModel.Runtime.InvokeMonitor<SafetyMonitor>(new SafetyMonitor.NotifyLeaderElected(this.CurrentTerm));

            this.ClusterManager.NotifyLeaderUpdate(this.ServerId, this.CurrentTerm);

            var logIndex = this.Logs.Count;
            var logTerm = this.GetLogTermForIndex(logIndex);

            this.NextIndex.Clear();
            this.MatchIndex.Clear();
            foreach (var server in this.Servers)
            {
                if (server.Key == this.ServerId)
                    continue;
                this.NextIndex.Add(server.Value, logIndex + 1);
                this.MatchIndex.Add(server.Value, 0);
            }

            foreach (var server in this.Servers)
            {
                if (server.Key == this.ServerId)
                    continue;
                //this.Send(this.Servers[idx], new AppendEntriesRequest(this.CurrentTerm, this.Id,
                //    logIndex, logTerm, new List<Log>(), this.CommitIndex, null));
            }
        }

        private void BroadcastVoteRequests()
        {
            // BUG: duplicate votes from same follower
            //if (ActorModel.Random())
            //{
                if (this.PeriodicTimer != null)
                {
                    this.PeriodicTimer.Dispose();
                }

                this.PeriodicTimer = this.RegisterTimer(RebroadcastVoteRequests, null,
                    TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            //}

            foreach (var server in this.Servers)
            {
                if (server.Key == this.ServerId)
                    continue;

                var lastLogIndex = this.Logs.Count;
                var lastLogTerm = this.GetLogTermForIndex(lastLogIndex);

                ActorModel.Log($"<RaftLog> Server {this.ServerId} sending vote request " +
                    $"to server {server.Key}.");
                server.Value.VoteRequest(this.CurrentTerm, this.ServerId, lastLogIndex, lastLogTerm);
            }
        }

        public Task VoteRequest(int term, int candidateId, int lastLogIndex, int lastLogTerm)
        {
            if (this.Role == Role.Follower)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;
                }

                this.ProcessVoteRequest(term, candidateId, lastLogIndex, lastLogTerm);
            }
            else if (this.Role == Role.Candidate)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;

                    this.ProcessVoteRequest(term, candidateId, lastLogIndex, lastLogTerm);

                    this.BecomeFollower();
                }
                else
                {
                    this.ProcessVoteRequest(term, candidateId, lastLogIndex, lastLogTerm);
                }
            }
            else if (this.Role == Role.Leader)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;

                    //this.RedirectLastClientRequestToClusterManager();
                    this.ProcessVoteRequest(term, candidateId, lastLogIndex, lastLogTerm);

                    this.BecomeFollower();
                }
                else
                {
                    this.ProcessVoteRequest(term, candidateId, lastLogIndex, lastLogTerm);
                }
            }

            return new Task(() => { });
        }

        void ProcessVoteRequest(int term, int candidateId, int lastLogIndex, int lastLogTerm)
        {
            var candidate = GrainClient.GrainFactory.GetGrain<IServer>(candidateId);

            if (term < this.CurrentTerm ||
                (this.VotedForCandidate != null && this.VotedForCandidate != candidate) ||
                this.Logs.Count > lastLogIndex ||
                this.GetLogTermForIndex(this.Logs.Count) > lastLogTerm)
            {
                ActorModel.Log($"<RaftLog> Server {this.ServerId} | term {this.CurrentTerm} " +
                    $"| log {this.Logs.Count} | vote granted {false}.");
                candidate.VoteResponse(this.CurrentTerm, false);
            }
            else
            {
                this.VotedForCandidate = candidate;
                this.Leader = null;

                ActorModel.Log($"<RaftLog> Server {this.ServerId} | term {this.CurrentTerm} " +
                    $"| log {this.Logs.Count} | vote granted {true}.");
                candidate.VoteResponse(this.CurrentTerm, true);
            }
        }

        public Task VoteResponse(int term, bool voteGranted)
        {
            if (this.Role == Role.Follower)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;
                }
            }
            else if (this.Role == Role.Candidate)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;

                    this.BecomeFollower();
                }
                else if (term != this.CurrentTerm)
                {
                    new Task(() => { });
                }

                if (voteGranted)
                {
                    this.VotesReceived++;
                    if (this.VotesReceived >= (this.Servers.Count / 2) + 1)
                    {
                        this.BecomeLeader();
                    }
                }
            }
            else if (this.Role == Role.Leader)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;

                    //this.RedirectLastClientRequestToClusterManager();
                    this.BecomeFollower();
                }
            }

            return new Task(() => { });
        }

        private Task StartLeaderElection(object args)
        {
            this.BecomCandidate();
            return new Task(() => { });
        }

        private Task RebroadcastVoteRequests(object args)
        {
            this.BroadcastVoteRequests();
            return new Task(() => { });
        }

        int GetLogTermForIndex(int logIndex)
        {
            var logTerm = 0;
            if (logIndex > 0)
            {
                logTerm = this.Logs[logIndex - 1].Term;
            }

            return logTerm;
        }

        #endregion
    }
}
