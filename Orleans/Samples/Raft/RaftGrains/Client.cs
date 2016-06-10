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

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random Random;

        private int LatestCommand;

        /// <summary>
        /// The request timer.
        /// </summary>
        IDisposable RequestTimer;

        #endregion

        #region methods
        
        public override Task OnActivateAsync()
        {
            ActorModel.Log($"<RaftLog> Client is activating.");

            if (this.LatestCommand <= 0)
            {
                this.Random = new Random(DateTime.Now.Millisecond);
                this.LatestCommand = -1;
            }

            return base.OnActivateAsync();
        }

        public Task Configure(int clusterId)
        {
            if (this.ClusterManager == null)
            {
                ActorModel.Log($"<RaftLog> Client is configuring.");

                this.ClusterManager = this.GrainFactory.GetGrain<IClusterManager>(clusterId);

                this.RequestTimer = this.RegisterTimer(PumpRequest, null,
                    TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            }

            return TaskDone.Done;
        }

        private Task PumpRequest(object args)
        {
            if (this.RequestTimer != null)
            {
                this.RequestTimer.Dispose();
                this.RequestTimer = null;
            }

            this.LatestCommand = new Random().Next(100);

            ActorModel.Log($"<RaftLog> Client is sending new request {this.LatestCommand}.");

            this.ClusterManager.RelayClientRequest(6, this.LatestCommand);

            return TaskDone.Done;
        }

        public Task ProcessResponse()
        {
            ActorModel.Log($"<RaftLog> Client received a response.");

            if (this.RequestTimer != null)
            {
                this.RequestTimer.Dispose();
                this.RequestTimer = null;
            }

            this.RequestTimer = this.RegisterTimer(PumpRequest, null,
                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            return TaskDone.Done;
        }

        #endregion
    }
}
