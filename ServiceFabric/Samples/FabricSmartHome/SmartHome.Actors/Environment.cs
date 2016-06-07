using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;

using SmartHome.Interfaces;

namespace SmartHome.Actors
{
    internal class Environment : Actor, IEnvironment
    {
        private IActorTimer PersonTimer;
        private IActorTimer ThiefTimer;

        private List<IPerson> People;
        private IThief Thief;

        async Task IEnvironment.Start()
        {
            await Task.Run(() =>
            {
                if (this.People == null)
                {
                    this.People = new List<IPerson>();

                    this.Thief = ActorProxy.Create<IThief>(new ActorId(1), "fabric:/FabricSmartHome");

                    this.People.Add(ActorProxy.Create<IPerson>(new ActorId(2), "fabric:/FabricSmartHome"));
                    this.People.Add(ActorProxy.Create<IPerson>(new ActorId(3), "fabric:/FabricSmartHome"));
                    this.People.Add(ActorProxy.Create<IPerson>(new ActorId(4), "fabric:/FabricSmartHome"));

                    ActorProxy.Create<IHouse>(new ActorId(5), "fabric:/FabricSmartHome");

                    ActorEventSource.Current.ActorMessage(this, "[LOG] Environment started.");

                    this.PersonTimer = this.RegisterTimer(HandlePersonTimeout, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));
                    this.ThiefTimer = this.RegisterTimer(HandleThiefTimeout, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));
                }
            });
        }

        public async Task HandlePersonTimeout(object args)
        {
            await Task.Run(() =>
            {
                this.UnregisterTimer(this.PersonTimer);
                ActorEventSource.Current.ActorMessage(this, "[LOG] People enter the house.");
                foreach (var person in this.People)
                {
                    person.Enter(Location.House);
                }
            });
        }

        public async Task HandleThiefTimeout(object args)
        {
            await Task.Run(() =>
            {
                this.UnregisterTimer(this.ThiefTimer);
                ActorEventSource.Current.ActorMessage(this, "[LOG] Thief enters the house.");
                this.Thief.Enter(Location.House);
            });
        }

        protected override async Task OnDeactivateAsync()
        {
            this.UnregisterTimer(this.PersonTimer);
            this.UnregisterTimer(this.ThiefTimer);

            await base.OnDeactivateAsync();
        }
    }
}
