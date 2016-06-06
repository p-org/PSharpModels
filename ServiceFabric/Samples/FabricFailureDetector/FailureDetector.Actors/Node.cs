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

        private HashSet<ulong> ProcessedPingIds;

        #endregion

        #region methods

        public Task Configure(int id)
        {
            if (this.ProcessedPingIds == null)
            {
                this.NodeId = id;
                this.IsHalted = false;
                this.ProcessedPingIds = new HashSet<ulong>();
            }

            return new Task(() => { });
        }

        public Task Ping(ulong pingId, int senderId)
        {
            if (this.IsHalted || this.ProcessedPingIds.Contains(pingId))
            {
                return new Task(() => { });
            }

            this.ProcessedPingIds.Add(pingId);

            var sender = ActorProxy.Create<IFailureDetector>(
                new ActorId(senderId), "FailureDetectorProxy");

            //ActorModel.Runtime.InvokeMonitor<SafetyMonitor>(new SafetyMonitor.NotifyPong(this.NodeId));

            sender.Pong(this.NodeId);

            return new Task(() => { });
        }

        public Task Halt()
        {
            this.IsHalted = true;
            return new Task(() => { });
        }

        #endregion
    }
}
