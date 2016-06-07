﻿using System;
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
        protected override Task OnActivateAsync()
        {
            ActorProxy.Create<IGarden>(new ActorId(6), "fabric:/FabricSmartHome");
            ActorProxy.Create<IKitchen>(new ActorId(7), "fabric:/FabricSmartHome");
            ActorProxy.Create<IBedroom>(new ActorId(8), "fabric:/FabricSmartHome");

            return base.OnActivateAsync();
        }

        public Task<Location> GotoRoom()
        {
            var roomId = new Random().Next(3);
            if (roomId == 0)
            {
                return Task.FromResult(Location.Garden);
            }
            else if (roomId == 0)
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
