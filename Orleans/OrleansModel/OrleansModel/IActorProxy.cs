namespace Microsoft.ServiceFabric.Actors.Client
{
    //
    // Summary:
    //     Provides the interface for implementation of proxy access for actor service.
    public interface IActorProxy
    {
        //
        // Summary:
        //     Gets Microsoft.ServiceFabric.Actors.ActorId associated with the proxy object.
        ActorId ActorId { get; }
        //
        // Summary:
        //     Gets Microsoft.ServiceFabric.Actors.Client.IActorServicePartitionClient that
        //     this proxy is using to communicate with the actor.
        //IActorServicePartitionClient ActorServicePartitionClient { get; }
    }
}
