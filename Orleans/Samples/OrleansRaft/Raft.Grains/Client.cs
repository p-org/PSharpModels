using System;
using System.Threading.Tasks;

using Orleans;
using Orleans.Concurrency;

using Raft.Interfaces;

namespace Raft.Grains
{
    /// <summary>
    /// Grain implementation class Client.
    /// </summary>
    [Reentrant]
    public class Client : Grain<int>, IClient
    {
        #region fields

        private IClusterManager ClusterManager;

        private int LatestCommand;
        private int Counter;

        #endregion

        #region methods

        public override Task OnActivateAsync()
        {
            Console.WriteLine($"<RaftLog> Client is activating.");

            if (this.LatestCommand <= 0)
            {
                this.LatestCommand = -1;
                this.Counter = 0;
            }

            return base.OnActivateAsync();
        }

        public Task Configure(int clusterId)
        {
            if (this.ClusterManager == null)
            {
                Console.WriteLine($"<RaftLog> Client is configuring.");

                this.ClusterManager = GrainClient.GrainFactory.
                GetGrain<IClusterManager>(clusterId);
            }
            
            return new Task(() => { });
        }

        #endregion
    }
}
