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

        public override Task OnActivateAsync()
        {
            if (this.Servers == null)
            {
                this.NumberOfServers = 5;
                this.LeaderTerm = 0;

                this.Servers = new Dictionary<int, IServer>();
                for (int idx = 1; idx <= this.NumberOfServers; idx++)
                {
                    this.Servers.Add(idx, GrainClient.GrainFactory.GetGrain<IServer>(idx));
                }

                this.Client = GrainClient.GrainFactory.GetGrain<IClient>(6);
            }

            return base.OnActivateAsync();
        }

        public Task Configure()
        {
            var serverIds = new List<int>(this.Servers.Keys);
            for (int idx = 1; idx <= this.NumberOfServers; idx++)
            {
                var serverTask = this.Servers[idx].Configure(idx, serverIds, 0);
                ActorModel.Wait(serverTask);
            }

            var clientTask = this.Client.Configure(0);
            ActorModel.Wait(clientTask);

            return new Task(() => { });
        }

        public Task NotifyLeaderUpdate(int leaderId, int term)
        {
            if (this.LeaderTerm < term)
            {
                this.Leader = GrainClient.GrainFactory.GetGrain<IServer>(leaderId);
                this.LeaderTerm = term;
            }

            return new Task(() => { });
        }

        #endregion
    }
}
