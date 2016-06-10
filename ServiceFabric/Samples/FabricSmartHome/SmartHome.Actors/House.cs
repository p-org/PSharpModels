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
    internal class House : Actor, IHouse
    {
        protected override async Task OnActivateAsync()
        {
            ActorProxy.Create<IGarden>(new ActorId(101), "fabric:/FabricSmartHome");
            ActorProxy.Create<IKitchen>(new ActorId(102), "fabric:/FabricSmartHome");
            ActorProxy.Create<IBedroom>(new ActorId(103), "fabric:/FabricSmartHome");

            await base.OnActivateAsync();
        }

        Task<Location> IHouse.GotoRoom()
        {
            var roomId = new Random().Next(3);
            if (roomId == 0)
            {
                return Task.FromResult(Location.Garden);
            }
            else if (roomId == 1)
            {
                return Task.FromResult(Location.Kitchen);
            }
            else
            {
                return Task.FromResult(Location.Bedroom);
            }
        }
    }
}
