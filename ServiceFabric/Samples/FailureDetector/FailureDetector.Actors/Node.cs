﻿using System;
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

        private HashSet<ulong> ProcessedPingIds;

        #endregion

        #region methods

        public Task Configure(int id)
        {
            if (this.ProcessedPingIds == null)
            {
                this.NodeId = id;
                this.ProcessedPingIds = new HashSet<ulong>();
            }

            return new Task(() => { });
        }

        public Task Ping(ulong pingId, int senderId)
        {
            if (this.ProcessedPingIds.Contains(pingId))
            {
                return new Task(() => { });
            }

            this.ProcessedPingIds.Add(pingId);

            var sender = ActorProxy.Create<IFailureDetector>(
                new ActorId(senderId), "FailureDetectorProxy");

            ActorModel.Runtime.InvokeMonitor<SafetyMonitor>(new SafetyMonitor.NotifyPong(this.NodeId));

            sender.Pong(this.NodeId);

            return new Task(() => { });
        }

        #endregion
    }
}