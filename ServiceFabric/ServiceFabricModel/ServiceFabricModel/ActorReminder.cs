using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricModel
{
    internal class ActorReminder : ReminderCancellationSource, IActorReminder
    {
        /// <summary>
        /// Name of this Reminder.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        public TimeSpan DueTime
        {
            get;
            private set;
        }

        public TimeSpan Period
        {
            get;
            private set;
        }

        public byte[] State
        {
            get;
            private set;
        }

        public ActorReminder(MachineId actor, MachineId reminder, string reminderName)
            : base(actor, reminder)
        {
            this.Name = reminderName;
        }
    }
}
