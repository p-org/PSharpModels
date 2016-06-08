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

        async Task<Location> IHouse.GotoRoom()
        {
            return await Task.Run(() =>
            {
                var roomId = new Random().Next(3);
                if (roomId == 0)
                {
                    return Location.Garden;
                }
                else if (roomId == 1)
                {
                    return Location.Kitchen;
                }
                else
                {
                    return Location.Bedroom;
                }
            });
        }
    }
}
