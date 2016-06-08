using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Orleans;
using Orleans.Concurrency;

using Raft.Interfaces;

namespace Raft.Grains
{
    /// <summary>
    /// Grain implementation class ClusterManager.
    /// </summary>
    [Reentrant]
    public class ClusterManager : Grain, IClusterManager
    {
        #region fields

        private IDictionary<int, IServer> Servers;
        private int NumberOfServers;

        private IServer Leader;
        private int LeaderTerm;

        private IClient Client;

        #endregion

        #region methods

        public override async Task OnActivateAsync()
        {
            if (this.Servers == null)
            {
                this.NumberOfServers = 5;
                this.LeaderTerm = 0;

                this.Servers = new Dictionary<int, IServer>();
                for (int idx = 1; idx <= this.NumberOfServers; idx++)
                {
                    Console.WriteLine($"<RaftLog> ClusterManager is creating server {idx}.");
                    this.Servers.Add(idx, this.GrainFactory.GetGrain<IServer>(idx));
                }

                this.Client = this.GrainFactory.GetGrain<IClient>(6);
            }

            await base.OnActivateAsync();
        }

        public async Task Configure()
        {
            var serverIds = new List<int>(this.Servers.Keys);
            for (int idx = 1; idx <= this.NumberOfServers; idx++)
            {
                Console.WriteLine($"<RaftLog> ClusterManager is configuring server {idx}.");
                await this.Servers[idx].Configure(idx, serverIds, 0);
            }

            await this.Client.Configure(0);
        }

        public async Task NotifyLeaderUpdate(int leaderId, int term)
        {
            await Task.Run(() =>
            {
                if (this.LeaderTerm < term)
                {
                    this.Leader = GrainClient.GrainFactory.GetGrain<IServer>(leaderId);
                    this.LeaderTerm = term;
                }
            });
        }

        #endregion
    }
}
