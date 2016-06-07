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
    public class Driver : Actor, IDriver
    {
        #region fields

        private ISafetyMonitor SafetyMonitor;
        private IFailureDetector FailureDetector;

        private Dictionary<int, INode> Nodes;
        private int NumberOfNodes;

        private Dictionary<INode, bool> NodeMap;

        private IActorTimer Timer;

        #endregion

        #region methods

        public async Task Start()
        {
            ActorEventSource.Current.ActorMessage(this, "[Driver] is starting");

            if (this.Nodes == null)
            {
                this.NumberOfNodes = 2;

                this.Nodes = new Dictionary<int, INode>();
                this.NodeMap = new Dictionary<INode, bool>();

                this.SafetyMonitor = ActorProxy.Create<ISafetyMonitor>(new ActorId(1), "fabric:/FabricFailureDetector");
                this.FailureDetector = ActorProxy.Create<IFailureDetector>(new ActorId(2), "fabric:/FabricFailureDetector");

                await this.Initialize();

                await this.FailureDetector.Configure(this.Nodes.Keys.ToList());
                await this.FailureDetector.RegisterClient(0);
                await this.FailureDetector.Start();

                //this.Fail();
            }
        }

        private async Task Initialize()
        {
            for (int idx = 0; idx < this.NumberOfNodes; idx++)
            {
                var node = ActorProxy.Create<INode>(new ActorId(idx + 3), "fabric:/FabricFailureDetector");
                await node.Configure(idx + 3);

                this.Nodes.Add(idx + 3, node);
                this.NodeMap.Add(node, true);
            }
        }

        private void Fail()
        {
            this.Timer = this.RegisterTimer(HandleTimeout, null,
                TimeSpan.FromSeconds(new Random().Next(10) + 5),
                TimeSpan.FromSeconds(new Random().Next(10) + 10));
        }

        public async Task HandleTimeout(object args)
        {
            await Task.Run(() =>
            {
                foreach (var node in this.Nodes)
                {
                    node.Value.Halt();
                }
            });
        }

        protected override async Task OnDeactivateAsync()
        {
            if (this.Timer != null)
            {
                this.UnregisterTimer(this.Timer);
                this.Timer = null;
            }

            await base.OnDeactivateAsync();
        }

        #endregion
    }
}
