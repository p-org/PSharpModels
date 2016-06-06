using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;

using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FailureDetector
{
    public class FailureDetector : Actor, IFailureDetector
    {
        #region fields

        private int FailureDetectorId;

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
                this.FailureDetectorId = 1;
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

            return new Task(() => { });
        }

        public Task Start()
        {
            this.SendPings();
            return new Task(() => { });
        }

        private void SendPings()
        {
            foreach (var node in this.Nodes)
            {
                if (this.Alive.ContainsKey(node) && !this.Responses.ContainsKey(node))
                {
                    ActorModel.Runtime.InvokeMonitor<SafetyMonitor>(
                        new SafetyMonitor.NotifyPing(this.FailureDetectorId));
                    node.Ping(this.FailureDetectorId);
                }
            }

            this.Timer = this.RegisterTimer(HandleTimeout, null,
                TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0));
        }

        public Task Pong(int senderId)
        {
            var node = ActorProxy.Create<INode>(
                new ActorId(senderId), "NodeProxy");

            if (this.Alive.ContainsKey(node))
            {
                this.Responses[node] = true;
                if (this.Responses.Count == this.Alive.Count)
                {
                    this.UnregisterTimer(this.Timer);
                }
            }
            
            return new Task(() => { });
        }

        public Task RegisterClient(int clientId)
        {
            var client = ActorProxy.Create<IDriver>(new ActorId(clientId), "DriverProxy");
            this.Clients[client] = true;

            return new Task(() => { });
        }

        public Task UnregisterClient(int clientId)
        {
            var client = ActorProxy.Create<IDriver>(new ActorId(clientId), "DriverProxy");
            if (this.Clients.ContainsKey(client))
            {
                this.Clients.Remove(client);
            }

            return new Task(() => { });
        }

        public Task HandleTimeout(object args)
        {
            this.Attempts++;
            if (this.Responses.Count < this.Alive.Count && this.Attempts < 2)
            {
                this.SendPings();
            }
            else
            {
                this.CheckAliveSet();
                this.Attempts = 0;
                this.Responses.Clear();
                this.SendPings();
            }

            return Task.FromResult(true);
        }

        private void CheckAliveSet()
        {
            foreach (var node in this.Nodes)
            {
                if (this.Alive.ContainsKey(node) && !this.Responses.ContainsKey(node))
                {
                    this.Alive.Remove(node);
                }
            }
        }

        #endregion
    }
}
