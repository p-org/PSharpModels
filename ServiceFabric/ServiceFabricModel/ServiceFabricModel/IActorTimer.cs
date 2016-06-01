using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using System;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    //
    // Summary:
    //     Represents Timer set on an Actor
    public interface IActorTimer 
    {
        //
        // Summary:
        //     Time when timer is first due.
        TimeSpan DueTime { get; }
        //
        // Summary:
        //     Periodic time when timer will be invoked.
        TimeSpan Period { get; }
    }
}
