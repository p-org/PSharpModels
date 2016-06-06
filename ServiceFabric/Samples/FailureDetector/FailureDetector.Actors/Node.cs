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

        #endregion

        #region methods

        public Task Configure(int id)
        {
            this.NodeId = id;
            return new Task(() => { });
        }

        public Task Ping(int senderId)
        {
            var sender = ActorProxy.Create<IFailureDetector>(
                new ActorId(senderId), "FailureDetectorProxy");

            ActorModel.Runtime.InvokeMonitor<SafetyMonitor>(new SafetyMonitor.NotifyPong(this.NodeId));

            sender.Pong(this.NodeId);

            return new Task(() => { });
        }

        #endregion
    }
}
