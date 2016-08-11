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

using Microsoft.PSharp.Actors;

namespace SmartHome.Actors
{
    public class Bedroom : Actor, IBedroom
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

        public async Task PersonEnters()
        {
            int numOfPeople = await this.StateManager.GetStateAsync<int>("PeopleInside");
            numOfPeople++;

            await this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
        }

        public async Task PersonExits()
        {
            int numOfPeople = await this.StateManager.GetStateAsync<int>("PeopleInside") - 1;
            if (numOfPeople < 0)
            {
                numOfPeople = 0;
            }

            await this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
        }

        public async Task AccessSafe()
        {
            bool isSafeOpen = await this.StateManager.GetStateAsync<bool>("IsSafeOpen");
            if (isSafeOpen)
            {
                ActorModel.Log("[LOG] The safe is closed.");
                await this.StateManager.SetStateAsync("IsSafeOpen", false);
            }
            else
            {
                ActorModel.Log("[LOG] The safe is open.");
                await this.StateManager.SetStateAsync("IsSafeOpen", true);
            }
            
        }

        public async Task<bool> TryEnterRoom()
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

        public async Task TryToStealMoney()
        {
            int numOfPeople = await this.StateManager.GetStateAsync<int>("PeopleInside");
            bool isSafeOpen = await this.StateManager.GetStateAsync<bool>("IsSafeOpen");
            ActorModel.Log("[LOG] Thief is searching for money. Room has {0} people", numOfPeople);

            ActorModel.Assert(!isSafeOpen || numOfPeople > 0, "Thief stole the money.");
        }
    }
}
