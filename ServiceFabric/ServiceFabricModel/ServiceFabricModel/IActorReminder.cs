using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    //
    // Summary:
    //     Encapsulates Reminder set on an Actor
    public interface IActorReminder
    {
        //
        // Summary:
        //     Time when Reminder is first due.
        TimeSpan DueTime { get; }
        //
        // Summary:
        //     Name of Reminder set on Actor. The name is unique per actor.
        string Name { get; }
        //
        // Summary:
        //     Periodic time when Reminder will be invoked.
        TimeSpan Period { get; }
        //
        // Summary:
        //     State to be passed into reminder invocation.
        byte[] State { get; }
    }
}
