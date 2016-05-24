using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class Actor<TState> : ActorBase where TState : class
    {
        public TState State { get; set; }
    }

    //
    // Summary:
    //     Represents a actor that can have multiple reliable 'named' states associated
    //     with it.
    //
    // Remarks:
    //     The state is preserved across actor garbage collections and fail-overs. The storage
    //     and retrieval of the state is provided by the actor state provider Microsoft.ServiceFabric.Actors.Runtime.IActorStateProvider.
    public abstract class Actor : ActorBase
    {
        //
        // Summary:
        //     Initializes a new instance of Microsoft.ServiceFabric.Actors.Runtime.Actor
        protected Actor()
        {

        }

        //
        // Summary:
        //     Gets the state manager for Microsoft.ServiceFabric.Actors.Runtime.Actor which
        //     can be used to get/add/update/remove named states.
        public IActorStateManager StateManager { get; set; }

        //
        // Summary:
        //     Saves all the state changes (add/update/remove) that were made since last call
        //     to Microsoft.ServiceFabric.Actors.Runtime.Actor.SaveStateAsync, to the actor
        //     state provider associated with the actor.
        //
        // Returns:
        //     A task that represents the asynchronous save operation.
        protected Task SaveStateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
