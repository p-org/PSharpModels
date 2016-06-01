using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using System;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    //
    // Summary:
    //     Represents Timer set on an Actor
    public class IActorTimer : TimerCancellationSource
    {
        //
        // Summary:
        //     Time when timer is first due.
        TimeSpan DueTime { get; }
        //
        // Summary:
        //     Periodic time when timer will be invoked.
        TimeSpan Period { get; }

        public IActorTimer(TimeSpan dueTime, TimeSpan period, MachineId actor, MachineId timer)
            :base(actor, timer)
        {
            this.DueTime = dueTime;
            this.Period = period;
        }
    }
}
