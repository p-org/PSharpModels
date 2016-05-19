using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using MyActor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    //[StatePersistence(StatePersistence.Persisted)]
    public class MyActor : Actor, IMyActor
    {
        private bool settingCount = false;

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            //ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see http://aka.ms/servicefabricactorsstateserialization

            await this.StateManager.TryAddStateAsync("count", 0);
        }

        Task<int> IMyActor.GetCountAsync()
        {
            return this.StateManager.GetStateAsync<int>("count");
        }

        /// <returns></returns>
        Task IMyActor.SetCountAsync(int count, List<IMyActor> next)
        {
            //return Task.Run(/*async*/ () =>
            //{
                this.StateManager.SetStateAsync("count", count).Wait();
                if (next != null && next.Count > 0)
                {
                    settingCount = true;
                    var actor = next[0];
                    Console.WriteLine("next actor: " + actor);
                    next.RemoveAt(0);
                    /*if (this.GetActorId().GetLongId() == 3) {*/ //await Task.Delay(1000); //}
                    actor.SetCountAsync(count + 1, next).Wait();
                    Console.WriteLine("inside setcountasync: " + actor.GetCountAsync().Result);
                    settingCount = false;
                }
            return Task.FromResult(true);
            //});
        }
    }
}
