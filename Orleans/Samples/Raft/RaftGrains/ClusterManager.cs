using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;

using Orleans;
using Orleans.Runtime;

namespace Raft
{
    /// <summary>
    /// Grain implementation class ClusterManager.
    /// </summary>
    [Reentrant]
    public class ClusterManager : Grain, IClusterManager
    {
        #region fields

        private IClusterManager Cluster;

        private IDictionary<int, IServer> Servers;
        private int NumberOfServers;

        private IServer Leader;
        private int LeaderTerm;

        private IClient Client;

        /// <summary>
        /// The retry timer.
        /// </summary>
        IDisposable RetryTimer;

        #endregion

        #region methods

        public override Task OnActivateAsync()
        {
            if (this.Servers == null)
            {
                this.NumberOfServers = 5;
                this.Leader = null;
                this.LeaderTerm = 0;

                this.Cluster = this.GrainFactory.GetGrain<IClusterManager>(0);

                this.Client = this.GrainFactory.GetGrain<IClient>(1);

                this.Servers = new Dictionary<int, IServer>();
                for (int idx = 0; idx < this.NumberOfServers; idx++)
                {
                    ActorModel.Log($"<RaftLog> ClusterManager is creating server {idx + 2}.");
                    this.Servers.Add(idx + 2, this.GrainFactory.GetGrain<IServer>(idx + 2));
                }
            }

            return base.OnActivateAsync();
        }

        public async Task Configure()
        {
            var serverIds = new List<int>(this.Servers.Keys);
            for (int idx = 0; idx < this.NumberOfServers; idx++)
            {
                Console.WriteLine($"<RaftLog> ClusterManager is configuring server {idx+2}.");
                var serverTask = this.Servers[idx+2].Configure(idx+2, serverIds, 0);
                await serverTask;
            }

            var clientTask = this.Client.Configure(0);
            await clientTask;
        }

        public Task NotifyLeaderUpdate(int leaderId, int term)
        {
            if (this.LeaderTerm < term)
            {
                ActorModel.Log($"<RaftLog> ClusterManager found new leader '{leaderId}'");

                this.Leader = this.GrainFactory.GetGrain<IServer>(leaderId);
                this.LeaderTerm = term;
            }

            return TaskDone.Done;
        }

        public Task RelayClientRequest(int clientId, int command)
        {
            ActorModel.Log($"<RaftLog> ClusterManager is relaying client request " + command + "\n");

            if (this.Leader != null)
            {
                this.Leader.ProcessClientRequest(clientId, command);
            }
            else
            {
                this.Cluster.RedirectClientRequest(clientId, command);
            }

            return TaskDone.Done;
        }

        public Task RedirectClientRequest(int clientId, int command)
        {
            this.RetryTimer = this.RegisterTimer(RedirectClientRequest, Tuple.Create(clientId, command),
                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            return TaskDone.Done;
        }

        private Task RedirectClientRequest(object args)
        {
            var request = (Tuple<int, int>)args;

            if (this.RetryTimer != null)
            {
                this.RetryTimer.Dispose();
                this.RetryTimer = null;
            }

            ActorModel.Log($"<RaftLog> ClusterManager is redirecting client request " + request.Item2 + "\n");

            this.Cluster.RelayClientRequest(request.Item1, request.Item2);

            return TaskDone.Done;
        }

        #endregion
    }
}
