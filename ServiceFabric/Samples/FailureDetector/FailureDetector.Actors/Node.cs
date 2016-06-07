using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;

using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FailureDetector
{
    public class Node : Actor, INode
    {
        #region methods

        private int NodeId;

        private HashSet<ulong> ProcessedRequests;

        #endregion

        #region methods

        public Task Configure(int id)
        {
            if (this.ProcessedRequests == null)
            {
                this.NodeId = id;
                this.ProcessedRequests = new HashSet<ulong>();
            }

            return new Task(() => { });
        }

        public Task Ping(ulong requestId, int senderId)
        {
            if (this.ProcessedRequests.Contains(requestId))
            {
                return new Task(() => { });
            }

            this.ProcessedRequests.Add(requestId);

            var sender = ActorProxy.Create<IFailureDetector>(
                new ActorId(senderId), "FailureDetectorProxy");

            ActorModel.Runtime.InvokeMonitor<SafetyMonitor>(new SafetyMonitor.NotifyPong(this.NodeId));

            sender.Pong(requestId, this.NodeId);

            return new Task(() => { });
        }

        #endregion
    }
}
