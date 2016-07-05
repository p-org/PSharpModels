using ServiceFabricModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// A P# actor reminder machine.
    /// </summary>
    public class ActorReminderMachine : ReminderMachine
    {
        protected override object CreateReminderCancellationSource()
        {
            return new ActorReminder(this.Target, this.Id, this.ReminderName);
        }
    }
}
