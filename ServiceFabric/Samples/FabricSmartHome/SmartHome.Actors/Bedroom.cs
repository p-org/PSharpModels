using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;

using SmartHome.Interfaces;

namespace SmartHome.Actors
{
    internal class Bedroom : Actor, IBedroom
    {
        protected override async Task OnActivateAsync()
        {
            if (!(await this.StateManager.ContainsStateAsync("IsSafeOpen")))
            {
                await this.StateManager.AddStateAsync("PeopleInside", 0);
                await this.StateManager.AddStateAsync("IsSafeOpen", false);
                await this.StateManager.AddStateAsync("Money", 1000);
            }

            await base.OnActivateAsync();
        }

        async Task IBedroom.PersonEnters()
        {
            int numOfPeople = await this.StateManager.GetStateAsync<int>("PeopleInside");
            numOfPeople++;

            await this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
        }

        async Task IBedroom.PersonExits()
        {
            int numOfPeople = await this.StateManager.GetStateAsync<int>("PeopleInside") - 1;
            if (numOfPeople < 0)
            {
                numOfPeople = 0;
            }

            await this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
        }

        async Task IBedroom.AccessSafe()
        {
            bool isSafeOpen = await this.StateManager.GetStateAsync<bool>("IsSafeOpen");
            if (isSafeOpen)
            {
                ActorEventSource.Current.ActorMessage(this, "[LOG] The safe is closed.");
                await this.StateManager.SetStateAsync("IsSafeOpen", false);
            }
            else
            {
                ActorEventSource.Current.ActorMessage(this, "[LOG] The safe is open.");
                await this.StateManager.SetStateAsync("IsSafeOpen", true);
            }
        }

        async Task<bool> IBedroom.TryEnterRoom()
        {
            int numOfPeople = await this.StateManager.GetStateAsync<int>("PeopleInside");
            if (numOfPeople > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        async Task IBedroom.TryToStealMoney()
        {
            int numOfPeople = await this.StateManager.GetStateAsync<int>("PeopleInside");
            bool isSafeOpen = await this.StateManager.GetStateAsync<bool>("IsSafeOpen");
            ActorEventSource.Current.ActorMessage(this, "[LOG] Thief is searching for money. Room has {0} people", numOfPeople);

            if (isSafeOpen && numOfPeople == 0)
            {
                ActorEventSource.Current.ActorMessage(this, "[LOG] Thief stole the money.");
                throw new InvalidOperationException("Thief stole the money.");
            }
        }
    }
}
