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
            var sender = (this.ReceivedEvent as NotifyPing).SenderId;

            if (!this.Pending.ContainsKey(sender))
            {
                this.Pending[sender] = 0;
            }

            this.Pending[sender] = this.Pending[sender] + 1;
            this.Assert(this.Pending[sender] <= 3, "1");
        }

        void MPongAction()
        {
            var node = (this.ReceivedEvent as NotifyPong).NodeId;

            this.Assert(this.Pending.ContainsKey(node), "2");
            this.Assert(this.Pending[node] > 0, "3");
            this.Pending[node] = this.Pending[node] - 1;
        }
    }
}
