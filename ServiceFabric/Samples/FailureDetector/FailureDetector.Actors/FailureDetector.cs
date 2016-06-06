using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FailureDetector
{
    public class FailureDetector : Actor, IFailureDetector
    {
        #region fields

        private List<INode> Nodes;

        private Dictionary<IDriver, bool> Clients;
        private int Attempts;

        private Dictionary<INode, bool> Alive;
        private Dictionary<INode, bool> Responses;

        private IActorTimer Timer;

        #endregion

        #region methods

        protected override Task OnActivateAsync()
        {
            if (this.Nodes == null)
            {
                this.Nodes = new List<INode>();
                this.Clients = new Dictionary<IDriver, bool>();
                this.Alive = new Dictionary<INode, bool>();
                this.Responses = new Dictionary<INode, bool>();
            }

            return base.OnActivateAsync();
        }

        public Task Configure(List<int> nodeIds)
        {
            foreach (var id in nodeIds)
            {
                this.Nodes.Add(ActorProxy.Create<INode>(new ActorId(id), "NodeProxy"));
            }
            
            foreach (var node in this.Nodes)
            {
                this.Alive.Add(node, true);
            }

            this.Timer = this.RegisterTimer(HandleTimeout, null,
                TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0));

            return new Task(() => { });
        }

        public Task RegisterClient(int clientId)
        {
            var client = ActorProxy.Create<IDriver>(new ActorId(clientId), "DriverProxy");
            this.Clients[client] = true;

            return new Task(() => { });
        }

        public Task HandleTimeout(object args)
        {
            Console.WriteLine("TIMED OUT!!!!");
            return Task.FromResult(true);
        }

        #endregion
    }
}
