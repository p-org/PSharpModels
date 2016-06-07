using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;

using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FailureDetector
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
                ActorModel.Wait(configureTask);

                var registerTask = this.FailureDetector.RegisterClient(0);
                ActorModel.Wait(registerTask);

                this.FailureDetector.Start();

                //this.Fail();
            }

            return base.OnActivateAsync();
        }

        private void Initialize()
        {
            for (int idx = 0; idx < this.NumberOfNodes; idx++)
            {
                var node = ActorProxy.Create<INode>(new ActorId(idx+2), "NodeProxy");
                var configureTask = node.Configure(idx + 2);
                ActorModel.Wait(configureTask);

                this.Nodes.Add(idx + 2, node);
                this.NodeMap.Add(node, true);
            }
        }

        private void Fail()
        {
            foreach (var node in this.Nodes)
            {
                ActorModel.Halt(node.Value as IPSharpActor);
            }
        }

        #endregion
    }
}
