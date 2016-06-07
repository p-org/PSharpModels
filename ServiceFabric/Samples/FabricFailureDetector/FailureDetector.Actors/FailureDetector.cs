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
    public class FailureDetector : Actor, IFailureDetector
    {
        #region fields

        private int FailureDetectorId;

        private Dictionary<int, INode> Nodes;

        private Dictionary<IDriver, bool> Clients;
        private int Attempts;

        private Dictionary<INode, bool> Alive;
        private Dictionary<INode, bool> Responses;

        private HashSet<ulong> ProcessedRequests;
        private ulong SendCounter;
        private ulong PingCounter;
        private bool HasStarted;

        private IActorTimer Timer;

        private ISafetyMonitor SafetyMonitor;

        #endregion

        #region methods

        protected override async Task OnActivateAsync()
        {
            if (this.Nodes == null)
            {
                this.FailureDetectorId = 2;
                this.Nodes = new Dictionary<int, INode>();
                this.Clients = new Dictionary<IDriver, bool>();
                this.Alive = new Dictionary<INode, bool>();
                this.Responses = new Dictionary<INode, bool>();
                this.ProcessedRequests = new HashSet<ulong>();
                this.PingCounter = 0;
                this.HasStarted = false;

                this.SafetyMonitor = ActorProxy.Create<ISafetyMonitor>(new ActorId(1), "fabric:/FabricFailureDetector");
            }

            await base.OnActivateAsync();
        }

        public async Task Configure(List<int> nodeIds)
        {
            await Task.Run(() =>
            {
                ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Creating {0} new nodes. Currently have {1} nodes.",
                    nodeIds.Count, this.Nodes.Count);

                if (this.Nodes.Count == 0)
                {
                    foreach (var id in nodeIds)
                    {
                        ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Creating new node {0}", id);
                        this.Nodes.Add(id, ActorProxy.Create<INode>(new ActorId(id), "fabric:/FabricFailureDetector"));
                    }

                    foreach (var node in this.Nodes)
                    {
                        this.Alive.Add(node.Value, true);
                    }
                }
            });
        }

        public async Task Start()
        {
            await Task.Run(async () =>
            {
                ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Starting");
                if (!this.HasStarted)
                {
                    this.HasStarted = true;
                    await this.SendPings();
                    ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Started");
                }
            });
        }

        private async Task SendPings()
        {
            this.SendCounter++;

            ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Wants to send some pings");
            foreach (var node in this.Nodes)
            {
                ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Wants to send ping to node {0}", node.Key);
                ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Alive {0}", this.Alive.ContainsKey(node.Value));
                ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Responses {0}", this.Responses.ContainsKey(node.Value));
                if (this.Alive.ContainsKey(node.Value) &&
                    !this.Responses.ContainsKey(node.Value))
                {
                    ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Sending ping to node {0}", node.Key);

                    ActorEventSource.Current.ActorMessage(this, "[Monitor] Notifies ping to node {0}", node.Key);
                    await SafetyMonitor.NotifyPing(node.Key);

                    await node.Value.Ping(PingCounter++, this.FailureDetectorId);
                }
            }

            this.Timer = this.RegisterTimer(HandleTimeout, null,
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
        }

        public async Task Pong(ulong requestId, int senderId)
        {
            await Task.Run(() =>
            {
                if (this.ProcessedRequests.Contains(requestId))
                {
                    return;
                }

                this.ProcessedRequests.Add(requestId);

                var node = ActorProxy.Create<INode>(new ActorId(senderId), "fabric:/FabricFailureDetector");

                ActorEventSource.Current.ActorMessage(this, "[FailureDetector] Received pong {0}", requestId);

                if (this.Alive.ContainsKey(node))
                {
                    this.Responses[node] = true;
                    if (this.Responses.Count == this.Alive.Count)
                    {
                        this.UnregisterTimer(this.Timer);
                    }
                }
            });
        }

        public async Task RegisterClient(int clientId)
        {
            await Task.Run(() =>
            {
                var client = ActorProxy.Create<IDriver>(new ActorId(clientId), "fabric:/FabricFailureDetector");
                this.Clients[client] = true;
            });
        }

        public async Task UnregisterClient(int clientId)
        {
            await Task.Run(() =>
            {
                var client = ActorProxy.Create<IDriver>(new ActorId(clientId), "fabric:/FabricFailureDetector");
                if (this.Clients.ContainsKey(client))
                {
                    this.Clients.Remove(client);
                }
            });
        }

        public async Task HandleTimeout(object args)
        {
            await Task.Run(async () =>
            {
                if (this.SendCounter > 5)
                {
                    this.UnregisterTimer(this.Timer);
                    return;
                }

                this.Attempts++;
                if (this.Responses.Count < this.Alive.Count && this.Attempts < 2)
                {
                    await this.SendPings();
                }
                else
                {
                    //this.CheckAliveSet();
                    this.Attempts = 0;
                    this.Responses.Clear();
                    await this.SendPings();
                }
            });
        }

        private void CheckAliveSet()
        {
            foreach (var node in this.Nodes)
            {
                if (this.Alive.ContainsKey(node.Value) &&
                    !this.Responses.ContainsKey(node.Value))
                {
                    this.Alive.Remove(node.Value);
                }
            }
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
