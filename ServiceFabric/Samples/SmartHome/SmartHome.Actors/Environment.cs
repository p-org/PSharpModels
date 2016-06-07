using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;

using SmartHome.Interfaces;

using Microsoft.PSharp.Actors;

namespace SmartHome.Actors
{
    public class Environment : Actor, IEnvironment
    {
        private IActorTimer PersonTimer;
        private IActorTimer ThiefTimer;

        private List<IPerson> People;
        private IThief Thief;

        public Task Start()
        {
            if (this.People == null)
            {
                this.People = new List<IPerson>();

                this.Thief = ActorProxy.Create<IThief>(new ActorId(1), "fabric:/FabricSmartHome");

                this.People.Add(ActorProxy.Create<IPerson>(new ActorId(2), "fabric:/FabricSmartHome"));
                this.People.Add(ActorProxy.Create<IPerson>(new ActorId(3), "fabric:/FabricSmartHome"));
                this.People.Add(ActorProxy.Create<IPerson>(new ActorId(4), "fabric:/FabricSmartHome"));

                ActorProxy.Create<IHouse>(new ActorId(5), "fabric:/FabricSmartHome");

                ActorModel.Log("[LOG] Environment started.");

                this.PersonTimer = this.RegisterTimer(HandlePersonTimeout, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));
                this.ThiefTimer = this.RegisterTimer(HandleThiefTimeout, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));
            }

            return new Task(() => { });
        }

        public Task HandlePersonTimeout(object args)
        {
            this.UnregisterTimer(this.PersonTimer);
            ActorModel.Log("[LOG] People enter the house.");
            foreach (var person in this.People)
            {
                person.Enter(Location.House);
            }

            return new Task(() => { });
        }

        public Task HandleThiefTimeout(object args)
        {
            this.UnregisterTimer(this.ThiefTimer);
            ActorModel.Log("[LOG] Thief enters the house.");
            this.Thief.Enter(Location.House);

            return new Task(() => { });
        }

        protected override Task OnDeactivateAsync()
        {
            if (this.PersonTimer != null)
            {
                this.UnregisterTimer(this.PersonTimer);
            }

            if (this.ThiefTimer != null)
            {
                this.UnregisterTimer(this.ThiefTimer);
            }

            return base.OnDeactivateAsync();
        }
    }
}
