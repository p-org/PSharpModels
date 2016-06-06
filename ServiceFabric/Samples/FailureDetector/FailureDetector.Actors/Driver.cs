using System;
using System.Collections.Generic;
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

        private List<INode> NodeSeq;
        private int NumberOfNodes;

        private Dictionary<INode, bool> NodeMap;

        #endregion

        #region methods

        protected override Task OnActivateAsync()
        {
            if (this.NodeSeq == null)
            {
                this.NumberOfNodes = 2;

                this.NodeSeq = new List<INode>();
                this.NodeMap = new Dictionary<INode, bool>();

                this.FailureDetector = ActorProxy.Create<IFailureDetector>(
                    new ActorId(1), "FailureDetectorProxy");

                this.Initialize();

                //this.Send(this.FailureDetector, new FailureDetector.Config(this.NodeSeq));
                //this.Send(this.FailureDetector, new RegisterClient(this.Id));

                this.Fail();
            }

            return base.OnActivateAsync();
        }

        private void Initialize()
        {
            for (int idx = 0; idx < this.NumberOfNodes; idx++)
            {
                var node = ActorProxy.Create<INode>(new ActorId(idx+2), "NodeProxy");
                this.NodeSeq.Add(node);
                this.NodeMap.Add(node, true);
            }
        }

        private void Fail()
        {
            foreach (var node in this.NodeSeq)
            {
                ActorModel.Halt(node as IPSharpActor);
            }

            //for (int i = 0; i < 2; i++)
            //{
            //    this.Send(this.NodeSeq[i], new Halt());
            //}
        }

        #endregion
    }
}
