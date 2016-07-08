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
    public class House : Actor, IHouse
    {
        protected override async Task OnActivateAsync()
        {
            ActorProxy.Create<IGarden>(new ActorId(101), "fabric:/FabricSmartHome");
            ActorProxy.Create<IKitchen>(new ActorId(102), "fabric:/FabricSmartHome");
            ActorProxy.Create<IBedroom>(new ActorId(103), "fabric:/FabricSmartHome");

            await base.OnActivateAsync();
        }

        public Task<Location> GotoRoom()
        {
            if (ActorModel.Random())
            {
                return Task.FromResult(Location.Garden);
            }
            else if (ActorModel.Random())
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
