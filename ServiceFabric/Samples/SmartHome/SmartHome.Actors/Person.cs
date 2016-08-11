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
    public class Person : Actor, IPerson, IRemindable
    {
        private IHouse House;
        private IGarden Garden;
        private IKitchen Kitchen;
        private IBedroom Bedroom;

        private IActorTimer MovementTimer;
        private IActorTimer ActionTimer;

        protected override Task OnActivateAsync()
        {
            if (!(this.StateManager.ContainsStateAsync("CurrentLocation").Result))
            {
                this.StateManager.AddStateAsync("CurrentLocation", Location.Outside);

                this.House = ActorProxy.Create<IHouse>(new ActorId(100), "fabric:/FabricSmartHome");
                this.Garden = ActorProxy.Create<IGarden>(new ActorId(101), "fabric:/FabricSmartHome");
                this.Kitchen = ActorProxy.Create<IKitchen>(new ActorId(102), "fabric:/FabricSmartHome");
                this.Bedroom = ActorProxy.Create<IBedroom>(new ActorId(103), "fabric:/FabricSmartHome");

                this.RegisterReminderAsync("HandleMovementTimeout", null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));
                this.RegisterReminderAsync("HandleActionTimeout", null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));
            }
            
            return base.OnActivateAsync();
        }

        public Task Enter(Location location)
        {
            this.StateManager.SetStateAsync("CurrentLocation", location);
            return new Task(() => { });
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName.Equals("HandleMovementTimeout"))
            {
                var previousLocation = ActorModel.GetResult(this.StateManager.GetStateAsync<Location>("CurrentLocation"));
                var location = ActorModel.GetResult(this.House.GotoRoom());

                ActorModel.Log("[LOG] Person entered room {0}", location);
                ActorModel.Wait(this.StateManager.SetStateAsync("CurrentLocation", location));

                if (previousLocation == Location.Garden)
                {
                    ActorModel.Wait(this.Garden.PersonExits());
                }
                else if (previousLocation == Location.Kitchen)
                {
                    ActorModel.Wait(this.Kitchen.PersonExits());
                }
                else if (previousLocation == Location.Bedroom)
                {
                    ActorModel.Wait(this.Bedroom.PersonExits());
                }

                if (location == Location.Garden)
                {
                    ActorModel.Wait(this.Garden.PersonEnters());
                }
                else if (location == Location.Kitchen)
                {
                    ActorModel.Wait(this.Kitchen.PersonEnters());
                }
                else if (location == Location.Bedroom)
                {
                    ActorModel.Wait(this.Bedroom.PersonEnters());
                }
            }
            else if (reminderName.Equals("HandleActionTimeout"))
            {
                var location = ActorModel.GetResult(this.StateManager.GetStateAsync<Location>("CurrentLocation"));
                if (location == Location.Garden)
                {

                }
                else if (location == Location.Kitchen)
                {

                }
                else if (location == Location.Bedroom)
                {
                    ActorModel.Wait(this.Bedroom.AccessSafe());
                }
            }
            return Task.FromResult(true);
        }

        protected override Task OnDeactivateAsync()
        {
            return base.OnDeactivateAsync();
        }
    }
}
