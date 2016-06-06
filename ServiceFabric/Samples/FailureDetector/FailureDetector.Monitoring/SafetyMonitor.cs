using System;
using System.Collections.Generic;
using Microsoft.PSharp;

namespace FailureDetector
{
    public class SafetyMonitor : Monitor
    {
        public class NotifyPing : Event
        {
            public int SenderId;

            public NotifyPing(int senderId)
                : base()
            {
                this.SenderId = senderId;
            }
        }

        public class NotifyPong : Event
        {
            public int NodeId;

            public NotifyPong(int nodeId)
                : base()
            {
                this.NodeId = nodeId;
            }
        }

        Dictionary<int, int> Pending;

        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventDoAction(typeof(NotifyPing), nameof(MPingAction))]
        [OnEventDoAction(typeof(NotifyPong), nameof(MPongAction))]
        class Init : MonitorState { }

        void InitOnEntry()
        {
            this.Pending = new Dictionary<int, int>();
        }

        void MPingAction()
        {
            var senderId = (this.ReceivedEvent as NotifyPing).SenderId;

            if (!this.Pending.ContainsKey(senderId))
            {
                this.Pending[senderId] = 0;
            }

            this.Pending[senderId] = this.Pending[senderId] + 1;
            this.Assert(this.Pending[senderId] <= 3, "Pending set " +
                $"for '{senderId}' contains more than 3 requests.");
        }

        void MPongAction()
        {
            var nodeId = (this.ReceivedEvent as NotifyPong).NodeId;

            this.Assert(this.Pending.ContainsKey(nodeId), "Pending " +
                $"set does not contain '{nodeId}'.");
            this.Assert(this.Pending[nodeId] > 0, "Pending set " +
                $"for '{nodeId}' is empty.");
            this.Pending[nodeId] = this.Pending[nodeId] - 1;
        }
    }
}
