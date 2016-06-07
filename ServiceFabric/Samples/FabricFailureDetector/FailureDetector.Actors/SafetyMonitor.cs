using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;

using FailureDetector.Interfaces;

namespace FailureDetector.Actors
{
    public class SafetyMonitor : Actor, ISafetyMonitor
    {
        private Dictionary<int, int> Pending;
        
        protected override async Task OnActivateAsync()
        {
            if (this.Pending == null)
            {
                this.Pending = new Dictionary<int, int>();
            }

            await base.OnActivateAsync();
        }

        public async Task NotifyPing(int senderId)
        {
            await Task.Run(() =>
            {
                if (!this.Pending.ContainsKey(senderId))
                {
                    this.Pending[senderId] = 0;
                }

                this.Pending[senderId] = this.Pending[senderId] + 1;

                ActorEventSource.Current.ActorMessage(this, "[Monitor] Notifies ping to node {0}", senderId);

                Contract.Assert(this.Pending[senderId] <= 3, "Pending set " +
                    $"for '{senderId}' contains more than 3 requests.");
            });
        }

        public async Task NotifyPong(int nodeId)
        {
            await Task.Run(() =>
            {
                ActorEventSource.Current.ActorMessage(this, "[Monitor] Notifies pong from node {0}", nodeId);

                Contract.Assert(this.Pending.ContainsKey(nodeId), "Pending " +
                $"set does not contain '{nodeId}'.");
                Contract.Assert(this.Pending[nodeId] > 0, "Pending set " +
                    $"for '{nodeId}' is empty.");
                this.Pending[nodeId] = this.Pending[nodeId] - 1;
            });
        }
    }
}
