using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.ServiceFabric.Actors
{
    //
    // Summary:
    //     Class containing extension methods for Actors.
    public static class ActorExtensions
    {
        //
        // Summary:
        //     Gets Microsoft.ServiceFabric.Actors.ActorId for the actor./>
        //
        // Parameters:
        //   actor:
        //     Actor object to get ActorId for.
        //
        // Type parameters:
        //   TIActor:
        //     Actor interface type.
        //
        // Returns:
        //     Microsoft.ServiceFabric.Actors.ActorId for the actor.
        public static ActorId GetActorId<TIActor>(this TIActor actor) where TIActor : IActor
        {
            var act = actor as Actor;
            return act.Id;
        }
        //
        // Summary:
        //     Gets Microsoft.ServiceFabric.Actors.ActorReference for the actor.
        //
        // Parameters:
        //   actor:
        //     Actor object to get ActorReference for.
        //
        // Returns:
        //     Microsoft.ServiceFabric.Actors.ActorReference for the actor.
        /*
        public static ActorReference GetActorReference(this IActor actor)
        {
            throw new NotImplementedException();
        }
        */
    }
}
