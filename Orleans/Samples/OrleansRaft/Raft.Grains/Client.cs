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

        public override async Task OnActivateAsync()
        {
            Console.WriteLine($"<RaftLog> Client is activating.");

            if (this.LatestCommand <= 0)
            {
                this.Random = new Random(DateTime.Now.Millisecond);
                this.LatestCommand = -1;
            }

            await base.OnActivateAsync();
        }

        public Task Configure(int clusterId)
        {
            if (this.ClusterManager == null)
            {
                Console.WriteLine($"<RaftLog> Client is configuring.");

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

            Console.WriteLine($"<RaftLog> Client is sending new request {this.LatestCommand}.");

            this.ClusterManager.RelayClientRequest(6, this.LatestCommand);

            return TaskDone.Done;
        }

        public Task ProcessResponse()
        {
            Console.WriteLine($"<RaftLog> Client received a response.");

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
