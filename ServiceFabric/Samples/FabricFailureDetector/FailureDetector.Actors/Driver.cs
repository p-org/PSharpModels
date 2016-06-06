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

        private IFailureDetector FailureDetector;

        private Dictionary<int, INode> Nodes;
        private int NumberOfNodes;

        private Dictionary<INode, bool> NodeMap;

        #endregion

        #region methods

        protected override Task OnActivateAsync()
        {
            if (this.Nodes == null)
            {
                this.NumberOfNodes = 2;

                this.Nodes = new Dictionary<int, INode>();
                this.NodeMap = new Dictionary<INode, bool>();

                this.FailureDetector = ActorProxy.Create<IFailureDetector>(
                    new ActorId(1), "FailureDetectorProxy");

                this.Initialize();

                var configureTask = this.FailureDetector.Configure(this.Nodes.Keys.ToList());
                configureTask.Wait();

                var registerTask = this.FailureDetector.RegisterClient(0);
                registerTask.Wait();

                this.FailureDetector.Start();

                this.Fail();
            }

            return base.OnActivateAsync();
        }

        private void Initialize()
        {
            for (int idx = 0; idx < this.NumberOfNodes; idx++)
            {
                var node = ActorProxy.Create<INode>(new ActorId(idx+2), "NodeProxy");
                var configureTask = node.Configure(idx + 2);
                configureTask.Wait();

                this.Nodes.Add(idx + 2, node);
                this.NodeMap.Add(node, true);
            }
        }

        private void Fail()
        {
            foreach (var node in this.Nodes)
            {
                node.Value.Halt();
            }
        }

        #endregion
    }
}
