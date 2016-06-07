using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;

using FailureDetector.Interfaces;

namespace FailureDetector.Actors
{
    public class Node : Actor, INode
    {
        #region methods

        private int NodeId;

        private bool IsHalted;

        private HashSet<ulong> ProcessedRequests;

        private ISafetyMonitor SafetyMonitor;

        #endregion

        #region methods

        public async Task Configure(int id)
        {
            await Task.Run(() =>
            {
                if (this.ProcessedRequests == null)
                {
                    this.NodeId = id;
                    this.IsHalted = false;
                    this.ProcessedRequests = new HashSet<ulong>();

                    this.SafetyMonitor = ActorProxy.Create<ISafetyMonitor>(
                        new ActorId(1), "fabric:/FabricFailureDetector");
                }
            });
        }

        public async Task Ping(ulong requestId, int senderId)
        {
            await Task.Run(async () =>
            {
                if (this.IsHalted || this.ProcessedRequests.Contains(requestId))
                {
                    return;
                }

                this.ProcessedRequests.Add(requestId);

                var sender = ActorProxy.Create<IFailureDetector>(
                    new ActorId(senderId), "fabric:/FabricFailureDetector");

                ActorEventSource.Current.ActorMessage(this, "[Node] Received ping {0}", requestId);

                ActorEventSource.Current.ActorMessage(this, "[Monitor] Notifies pong from node {0}", this.NodeId);
                await SafetyMonitor.NotifyPong(this.NodeId);

                await sender.Pong(requestId, this.NodeId);
            });
        }

        public async Task Halt()
        {
            await Task.Run(() =>
            {
                this.IsHalted = true;
            });
        }

        #endregion
    }
}
