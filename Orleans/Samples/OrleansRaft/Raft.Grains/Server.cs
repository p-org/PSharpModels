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
    public class Server : Grain<int>, IServer, IRemindable
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
        //IDisposable ElectionTimer;

        /// <summary>
        /// The election timer of this server.
        /// </summary>
        IGrainReminder ElectionReminder;

        /// <summary>
        /// The periodic timer of this server.
        /// </summary>
        //IDisposable PeriodicTimer;

        /// <summary>
        /// The periodic timer of this server.
        /// </summary>
        IGrainReminder PeriodicReminder;

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

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random Random;

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

        public async Task Configure(int id, List<int> serverIds, int clusterId)
        {
            await Task.Run(async () =>
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

                    await this.BecomeFollower();
                }
            });
        }

        private async Task BecomeFollower()
        {
            Console.WriteLine($"<RaftLog> Server {this.ServerId} became FOLLOWER.");
            this.Role = Role.Follower;

            this.Leader = null;
            this.VotesReceived = 0;

            //if (this.ElectionTimer != null)
            //{
            //    this.ElectionTimer.Dispose();
            //}

            //if (this.PeriodicTimer != null)
            //{
            //    this.PeriodicTimer.Dispose();
            //}

            if (this.ElectionReminder != null)
            {
                await this.UnregisterReminder(this.ElectionReminder);
                this.ElectionReminder = null;
            }

            if (this.PeriodicReminder != null)
            {
                await this.UnregisterReminder(this.PeriodicReminder);
                this.PeriodicReminder = null;
            }

            //if (ActorModel.Random())
            //{
            //this.ElectionTimer = this.RegisterTimer(StartLeaderElection, null,
            //        TimeSpan.FromSeconds(2 + this.Random.Next(10)), TimeSpan.FromSeconds(2 + this.Random.Next(10)));
            this.ElectionReminder = await this.RegisterOrUpdateReminder("StartLeaderElection",
                    TimeSpan.FromMinutes(2 + this.Random.Next(10)), TimeSpan.FromMinutes(2 + this.Random.Next(10)));
            //}
        }

        private async Task BecomeCandidate()
        {
            Console.WriteLine($"<RaftLog> Server {this.ServerId} became CANDIDATE.");
            this.Role = Role.Candidate;

            this.CurrentTerm++;
            this.VotedForCandidate = GrainClient.GrainFactory.GetGrain<IServer>(this.ServerId);
            this.VotesReceived = 1;

            Console.WriteLine($"<RaftLog> Candidate {this.ServerId} | term {this.CurrentTerm} " +
                $"| election votes {this.VotesReceived} | log {this.Logs.Count}.");

            await this.BroadcastVoteRequests();
        }

        private void BecomeLeader()
        {
            Console.WriteLine($"<RaftLog> Leader {this.ServerId} | term {this.CurrentTerm} " +
                $"| election votes {this.VotesReceived} | log {this.Logs.Count}.");

            this.VotesReceived = 0;

            //ActorModel.Runtime.InvokeMonitor<SafetyMonitor>(new SafetyMonitor.NotifyLeaderElected(this.CurrentTerm));

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

        private async Task BroadcastVoteRequests()
        {
            // BUG: duplicate votes from same follower
            //if (ActorModel.Random())
            //{
            //if (this.PeriodicTimer != null)
            //{
            //    this.PeriodicTimer.Dispose();
            //}

            if (this.PeriodicReminder != null)
            {
                await this.UnregisterReminder(this.PeriodicReminder);
                this.PeriodicReminder = null;
            }

            this.PeriodicReminder = await this.RegisterOrUpdateReminder("RebroadcastVoteRequests",
                TimeSpan.FromMinutes(2 + this.Random.Next(10)), TimeSpan.FromMinutes(2 + this.Random.Next(10)));

            //this.PeriodicTimer = this.RegisterTimer(RebroadcastVoteRequests, null,
            //        TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            //}

            foreach (var server in this.Servers)
            {
                if (server.Key == this.ServerId)
                    continue;

                var lastLogIndex = this.Logs.Count;
                var lastLogTerm = this.GetLogTermForIndex(lastLogIndex);

                Console.WriteLine($"<RaftLog> Server {this.ServerId} sending vote request " +
                    $"to server {server.Key}.");
                await server.Value.VoteRequest(this.CurrentTerm, this.ServerId, lastLogIndex, lastLogTerm);
            }
        }

        public async Task VoteRequest(int term, int candidateId, int lastLogIndex, int lastLogTerm)
        {
            await Task.Run(async () =>
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

                        await this.BecomeFollower();
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

                        await this.BecomeFollower();
                    }
                    else
                    {
                        this.ProcessVoteRequest(term, candidateId, lastLogIndex, lastLogTerm);
                    }
                }
            });
        }

        void ProcessVoteRequest(int term, int candidateId, int lastLogIndex, int lastLogTerm)
        {
            var candidate = GrainClient.GrainFactory.GetGrain<IServer>(candidateId);

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

        public async Task VoteResponse(int term, bool voteGranted)
        {
            await Task.Run(async () =>
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

                        await this.BecomeFollower();
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
                        await this.BecomeFollower();
                    }
                }
            });
        }

        //private Task StartLeaderElection(object args)
        //{
        //    this.BecomeCandidate();
        //    return TaskDone.Done;
        //}

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (reminderName.Equals("StartLeaderElection"))
            {
                await this.BecomeCandidate();
            }
            else if (reminderName.Equals("RebroadcastVoteRequests"))
            {
                await this.BroadcastVoteRequests();
            }
        }

        //private Task RebroadcastVoteRequests(object args)
        //{
        //    this.BroadcastVoteRequests();
        //    return TaskDone.Done;
        //}

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
