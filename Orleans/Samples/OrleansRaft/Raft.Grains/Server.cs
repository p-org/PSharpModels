using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;

using Raft.Interfaces;

namespace Raft.Grains
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
        /// The latest client id.
        /// </summary>
        int? LatestClientId;

        /// <summary>
        /// The latest client command.
        /// </summary>
        int? LatestClientCommand;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random Random;

        /// <summary>
        /// Safety monitor.
        /// </summary>
        private ISafetyMonitor SafetyMonitor;

        #endregion

        #region methods

        public override async Task OnActivateAsync()
        {
            Console.WriteLine($"<RaftLog> Server is activating.");

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

                this.Random = new Random(DateTime.Now.Millisecond);
            }
            
            await base.OnActivateAsync();
        }

        public Task Configure(int id, List<int> serverIds, int clusterId)
        {
            if (this.Servers.Count == 0)
            {
                this.ServerId = id;

                Console.WriteLine($"<RaftLog> Server {id} is configuring.");

                foreach (var idx in serverIds)
                {
                    this.Servers.Add(idx, this.GrainFactory.GetGrain<IServer>(idx));
                }

                this.ClusterManager = this.GrainFactory.GetGrain<IClusterManager>(clusterId);

                this.BecomeFollower();
            }

            return TaskDone.Done;
        }

        private void BecomeFollower()
        {
            Console.WriteLine($"<RaftLog> Server {this.ServerId} became FOLLOWER.");
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
            
            this.ElectionTimer = this.RegisterTimer(StartLeaderElection, null,
                TimeSpan.FromSeconds(5 + this.Random.Next(30)),
                TimeSpan.FromSeconds(5 + this.Random.Next(30)));
        }

        private void BecomeCandidate()
        {
            Console.WriteLine($"<RaftLog> Server {this.ServerId} became CANDIDATE.");
            this.Role = Role.Candidate;

            this.CurrentTerm++;
            this.VotedForCandidate = this.GrainFactory.GetGrain<IServer>(this.ServerId);
            this.VotesReceived = 1;

            Console.WriteLine($"<RaftLog> Candidate {this.ServerId} | term {this.CurrentTerm} " +
                $"| election votes {this.VotesReceived} | log {this.Logs.Count}.");

            this.BroadcastVoteRequests();
        }

        private async void BecomeLeader()
        {
            Console.WriteLine($"<RaftLog> Server {this.ServerId} became LEADER.");
            Console.WriteLine($"<RaftLog> Leader {this.ServerId} | term {this.CurrentTerm} " +
                $"| election votes {this.VotesReceived} | log {this.Logs.Count}.");

            this.Role = Role.Leader;
            this.VotesReceived = 0;

            this.SafetyMonitor = this.GrainFactory.GetGrain<ISafetyMonitor>(100);
            await this.SafetyMonitor.NotifyLeaderElected(this.CurrentTerm);

            await this.ClusterManager.NotifyLeaderUpdate(this.ServerId, this.CurrentTerm);

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

                await server.Value.AppendEntriesRequest(this.CurrentTerm, this.ServerId,
                    logIndex, logTerm, new List<Log>(), this.CommitIndex, -1);
            }
        }

        private void BroadcastVoteRequests()
        {
            // BUG: duplicate votes from same follower
            if (this.PeriodicTimer != null)
            {
                this.PeriodicTimer.Dispose();
            }
            
            this.PeriodicTimer = this.RegisterTimer(RebroadcastVoteRequests, null,
                TimeSpan.FromSeconds(5 + this.Random.Next(20)),
                TimeSpan.FromSeconds(5 + this.Random.Next(20)));

            foreach (var server in this.Servers)
            {
                if (server.Key == this.ServerId)
                    continue;

                var lastLogIndex = this.Logs.Count;
                var lastLogTerm = this.GetLogTermForIndex(lastLogIndex);

                Console.WriteLine($"<RaftLog> Server {this.ServerId} sending vote request " +
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

                    if (this.LatestClientId != null)
                    {
                        this.ClusterManager.RelayClientRequest(this.LatestClientId.Value,
                            this.LatestClientCommand.Value);
                    }

                    this.ProcessVoteRequest(term, candidateId, lastLogIndex, lastLogTerm);

                    this.BecomeFollower();
                }
                else
                {
                    this.ProcessVoteRequest(term, candidateId, lastLogIndex, lastLogTerm);
                }
            }

            return TaskDone.Done;
        }

        void ProcessVoteRequest(int term, int candidateId, int lastLogIndex, int lastLogTerm)
        {
            var candidate = this.GrainFactory.GetGrain<IServer>(candidateId);

            if (term < this.CurrentTerm ||
                (this.VotedForCandidate != null && this.VotedForCandidate != candidate) ||
                this.Logs.Count > lastLogIndex ||
                this.GetLogTermForIndex(this.Logs.Count) > lastLogTerm)
            {
                Console.WriteLine($"<RaftLog> Server {this.ServerId} | term {this.CurrentTerm} " +
                    $"| log {this.Logs.Count} | vote granted {false}.");
                candidate.VoteResponse(this.CurrentTerm, false);
            }
            else
            {
                this.VotedForCandidate = candidate;
                this.Leader = null;

                Console.WriteLine($"<RaftLog> Server {this.ServerId} | term {this.CurrentTerm} " +
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

                    if (this.LatestClientId != null)
                    {
                        this.ClusterManager.RelayClientRequest(this.LatestClientId.Value,
                            this.LatestClientCommand.Value);
                    }
                    
                    this.BecomeFollower();
                }
            }

            return TaskDone.Done;
        }

        public Task AppendEntriesRequest(int term, int leaderId, int prevLogIndex,
            int prevLogTerm, List<Log> entries, int leaderCommit, int clientId)
        {
            if (this.Role == Role.Follower)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;
                }

                this.AppendEntries(term, leaderId, prevLogIndex, prevLogTerm, entries,
                    leaderCommit, clientId);
            }
            else if (this.Role == Role.Candidate)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;

                    this.AppendEntries(term, leaderId, prevLogIndex, prevLogTerm,
                        entries, leaderCommit, clientId);
                    this.BecomeFollower();
                }
                else
                {
                    this.AppendEntries(term, leaderId, prevLogIndex, prevLogTerm,
                        entries, leaderCommit, clientId);
                }
            }
            else if (this.Role == Role.Leader)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;

                    if (this.LatestClientId != null)
                    {
                        this.ClusterManager.RelayClientRequest(this.LatestClientId.Value,
                            this.LatestClientCommand.Value);
                    }

                    this.AppendEntries(term, leaderId, prevLogIndex, prevLogTerm,
                        entries, leaderCommit, clientId);
                    this.BecomeFollower();
                }
            }

            return TaskDone.Done;
        }

        private void AppendEntries(int term, int leaderId, int prevLogIndex,
            int prevLogTerm, List<Log> entries, int leaderCommit, int clientId)
        {
            var leader = this.GrainFactory.GetGrain<IServer>(leaderId);

            if (term < this.CurrentTerm)
            {
                Console.WriteLine($"<RaftLog> Server {this.ServerId} | term {this.CurrentTerm} | log " +
                    $"{this.Logs.Count} | last applied {this.LastApplied} | append false (< term).");
                
                leader.AppendEntriesResponse(this.CurrentTerm, false, this.ServerId, clientId);
            }
            else
            {
                if (prevLogIndex > 0 &&
                    (this.Logs.Count < prevLogIndex ||
                    this.Logs[prevLogIndex - 1].Term != prevLogTerm))
                {
                    Console.WriteLine($"<RaftLog> Server {this.ServerId} | term {this.CurrentTerm} | log " +
                        $"{this.Logs.Count} | last applied {this.LastApplied} | append false (not in log).");
                    
                    leader.AppendEntriesResponse(this.CurrentTerm, false, this.ServerId, clientId);
                }
                else
                {
                    if (entries.Count > 0)
                    {
                        var currentIndex = prevLogIndex + 1;
                        foreach (var entry in entries)
                        {
                            if (this.Logs.Count < currentIndex)
                            {
                                this.Logs.Add(entry);
                            }
                            else if (this.Logs[currentIndex - 1].Term != entry.Term)
                            {
                                this.Logs.RemoveRange(currentIndex - 1, this.Logs.Count - (currentIndex - 1));
                                this.Logs.Add(entry);
                            }

                            currentIndex++;
                        }
                    }

                    if (leaderCommit > this.CommitIndex &&
                        this.Logs.Count < leaderCommit)
                    {
                        this.CommitIndex = this.Logs.Count;
                    }
                    else if (leaderCommit > this.CommitIndex)
                    {
                        this.CommitIndex = leaderCommit;
                    }

                    if (this.CommitIndex > this.LastApplied)
                    {
                        this.LastApplied++;
                    }

                    Console.WriteLine($"<RaftLog> Server {this.ServerId} | term {this.CurrentTerm} | log " +
                        $"{this.Logs.Count} | entries received {entries.Count} | last applied " +
                        $"{this.LastApplied} | append true.");

                    leader.AppendEntriesResponse(this.CurrentTerm, true, this.ServerId, clientId);
                }
            }
        }

        public Task AppendEntriesResponse(int term, bool success, int serverId, int clientId)
        {
            Console.WriteLine($"<RaftLog> Server {this.ServerId} | term {this.CurrentTerm} " +
                $"| append response {success}.");

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
            }
            else if (this.Role == Role.Leader)
            {
                if (term > this.CurrentTerm)
                {
                    this.CurrentTerm = term;
                    this.VotedForCandidate = null;

                    if (this.LatestClientId != null)
                    {
                        this.ClusterManager.RelayClientRequest(this.LatestClientId.Value,
                            this.LatestClientCommand.Value);
                    }

                    this.BecomeFollower();
                }
                else if (term != this.CurrentTerm)
                {
                    return TaskDone.Done;
                }

                var server = this.GrainFactory.GetGrain<IServer>(serverId);

                if (success)
                {
                    this.NextIndex[server] = this.Logs.Count + 1;
                    this.MatchIndex[server] = this.Logs.Count;

                    this.VotesReceived++;

                    if (clientId >= 0 &&
                        this.VotesReceived >= (this.Servers.Count / 2) + 1)
                    {
                        Console.WriteLine($"<RaftLog> Leader {this.ServerId} | term {this.CurrentTerm} " +
                            $"| append votes {this.VotesReceived} | append success.");

                        var commitIndex = this.MatchIndex[server];
                        if (commitIndex > this.CommitIndex &&
                            this.Logs[commitIndex - 1].Term == this.CurrentTerm)
                        {
                            this.CommitIndex = commitIndex;

                            Console.WriteLine($"<RaftLog> Leader {this.ServerId} | term {this.CurrentTerm} " +
                                $"| log {this.Logs.Count} | command {this.Logs[commitIndex - 1].Command}.");
                        }

                        this.VotesReceived = 0;
                        this.LatestClientId = null;
                        this.LatestClientCommand = null;

                        var client = this.GrainFactory.GetGrain<IClient>(clientId);
                        client.ProcessResponse();
                    }
                }
                else
                {
                    if (this.NextIndex[server] > 1)
                    {
                        this.NextIndex[server] = this.NextIndex[server] - 1;
                    }

                    var logs = this.Logs.GetRange(this.NextIndex[server] - 1,
                        this.Logs.Count - (this.NextIndex[server] - 1));

                    var prevLogIndex = this.NextIndex[server] - 1;
                    var prevLogTerm = this.GetLogTermForIndex(prevLogIndex);

                    Console.WriteLine($"<RaftLog> Leader {this.ServerId} | term {this.CurrentTerm} | log " +
                        $"{this.Logs.Count} | append votes {this.VotesReceived} " +
                        $"append fail (next idx = {this.NextIndex[server]}).");

                    server.AppendEntriesRequest(this.CurrentTerm, this.ServerId, prevLogIndex,
                        prevLogTerm, logs, this.CommitIndex, clientId);
                }
            }

            return TaskDone.Done;
        }

        public Task RedirectClientRequest(int clientId, int command)
        {
            if (this.Leader != null)
            {
                this.Leader.ProcessClientRequest(clientId, command);
            }
            else
            {
                this.ClusterManager.RedirectClientRequest(clientId, command);
            }

            return TaskDone.Done;
        }

        public Task ProcessClientRequest(int clientId, int command)
        {
            this.LatestClientId = clientId;
            this.LatestClientCommand = command;

            var log = new Log(this.CurrentTerm, command);
            this.Logs.Add(log);

            this.BroadcastLastClientRequest();

            return TaskDone.Done;
        }

        private void BroadcastLastClientRequest()
        {
            Console.WriteLine($"<RaftLog> Leader {this.ServerId}  sends append requests | term " +
                    $"{this.CurrentTerm} | log {this.Logs.Count}.");

            var lastLogIndex = this.Logs.Count;

            this.VotesReceived = 1;
            foreach (var server in this.Servers)
            {
                if (server.Key == this.ServerId)
                    continue;
                
                if (lastLogIndex < this.NextIndex[server.Value])
                    continue;

                var logs = this.Logs.GetRange(this.NextIndex[server.Value] - 1,
                    this.Logs.Count - (this.NextIndex[server.Value] - 1));

                var prevLogIndex = this.NextIndex[server.Value] - 1;
                var prevLogTerm = this.GetLogTermForIndex(prevLogIndex);

                server.Value.AppendEntriesRequest(this.CurrentTerm, this.ServerId, prevLogIndex,
                    prevLogTerm, logs, this.CommitIndex, this.LatestClientId.Value);
            }
        }

        private Task StartLeaderElection(object args)
        {
            this.BecomeCandidate();
            return TaskDone.Done;
        }

        private Task RebroadcastVoteRequests(object args)
        {
            this.BroadcastVoteRequests();
            return TaskDone.Done;
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
