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
    internal class Thief : Actor, IThief
    {
        private IHouse House;
        private IGarden Garden;
        private IKitchen Kitchen;
        private IBedroom Bedroom;

        private IActorTimer MovementTimer;
        private IActorTimer ActionTimer;

        protected override async Task OnActivateAsync()
        {
            if (!(await this.StateManager.ContainsStateAsync("CurrentLocation")))
            {
                await this.StateManager.AddStateAsync("CurrentLocation", Location.Outside);
            }

            this.House = ActorProxy.Create<IHouse>(new ActorId(100), "fabric:/FabricSmartHome");
            this.Garden = ActorProxy.Create<IGarden>(new ActorId(101), "fabric:/FabricSmartHome");
            this.Kitchen = ActorProxy.Create<IKitchen>(new ActorId(102), "fabric:/FabricSmartHome");
            this.Bedroom = ActorProxy.Create<IBedroom>(new ActorId(103), "fabric:/FabricSmartHome");

            this.MovementTimer = this.RegisterTimer(HandleMovementTimeout, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));
            this.ActionTimer = this.RegisterTimer(HandleActionTimeout, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));

            await base.OnActivateAsync();
        }

        async Task IThief.Enter(Location location)
        {
            await this.StateManager.SetStateAsync("CurrentLocation", location);
        }

        public async Task HandleMovementTimeout(object args)
        {
            var previousLocation = await this.StateManager.GetStateAsync<Location>("CurrentLocation");
            var location = await this.House.GotoRoom();
            ActorEventSource.Current.ActorMessage(this, "[LOG] Thief tries to enter room {0}", location);
            bool canEnter = false;
            if (location == Location.Garden)
            {
                canEnter = await this.Garden.TryEnterRoom();
            }
            else if (previousLocation == Location.Garden &&
                location == Location.Kitchen)
            {
                canEnter = await this.Kitchen.TryEnterRoom();
            }
            else if (previousLocation == Location.Kitchen &&
                location == Location.Bedroom)
            {
                canEnter = await this.Bedroom.TryEnterRoom();
            }

            if (canEnter)
            {
                ActorEventSource.Current.ActorMessage(this, "[LOG] Thief entered room {0}", location);
                await this.StateManager.SetStateAsync("CurrentLocation", location);
            }
        }

        public async Task HandleActionTimeout(object args)
        {
            var location = await this.StateManager.GetStateAsync<Location>("CurrentLocation");
            if (location == Location.Garden)
            {

            }
            else if (location == Location.Kitchen)
            {

            }
            else if (location == Location.Bedroom)
            {
                await this.Bedroom.TryToStealMoney();
            }
        }

        protected override async Task OnDeactivateAsync()
        {
            this.UnregisterTimer(this.MovementTimer);
            this.UnregisterTimer(this.ActionTimer);

            await base.OnDeactivateAsync();
        }
    }
}
