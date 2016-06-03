using System;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;

using Orleans;

namespace Raft
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
                this.ClusterManager = GrainClient.GrainFactory.
                GetGrain<IClusterManager>(clusterId);
            }
            
            return new Task(() => { });
        }

        #endregion
    }
}
