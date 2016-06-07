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
        protected override Task OnActivateAsync()
        {
            if (!this.StateManager.ContainsStateAsync("IsSafeOpen").Result)
            {
                this.StateManager.AddStateAsync("PeopleInside", 0);
                this.StateManager.AddStateAsync("IsSafeOpen", false);
                this.StateManager.AddStateAsync("Money", 1000);
            }

            return base.OnActivateAsync();
        }

        Task IBedroom.PersonEnters()
        {
            int numOfPeople = this.StateManager.GetStateAsync<int>("PeopleInside").Result;
            numOfPeople++;

            this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
            return new Task(() => { });
        }

        Task IBedroom.PersonExits()
        {
            int numOfPeople = this.StateManager.GetStateAsync<int>("PeopleInside").Result - 1;
            if (numOfPeople < 0)
            {
                numOfPeople = 0;
            }

            this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
            return new Task(() => { });
        }

        Task IBedroom.AccessSafe()
        {
            bool isSafeOpen = this.StateManager.GetStateAsync<bool>("IsSafeOpen").Result;
            if (isSafeOpen)
            {
                ActorModel.Log("[LOG] The safe is closed.");
                this.StateManager.SetStateAsync("IsSafeOpen", false);
            }
            else
            {
                ActorModel.Log("[LOG] The safe is open.");
                this.StateManager.SetStateAsync("IsSafeOpen", true);
            }

            return new Task(() => { });
        }

        Task<bool> IBedroom.TryEnterRoom()
        {
            int numOfPeople = this.StateManager.GetStateAsync<int>("PeopleInside").Result;
            if (numOfPeople > 0)
            {
                return Task.FromResult(false);
            }
            else
            {
                return Task.FromResult(true);
            }
        }

        Task IBedroom.TryToStealMoney()
        {
            int numOfPeople = this.StateManager.GetStateAsync<int>("PeopleInside").Result;
            bool isSafeOpen = this.StateManager.GetStateAsync<bool>("IsSafeOpen").Result;
            ActorModel.Log("[LOG] Thief is searching for money. Room has {0} people", numOfPeople);

            if (isSafeOpen && numOfPeople == 0)
            {
                ActorModel.Log("[LOG] Thief stole the money.");
                throw new InvalidOperationException("Thief stole the money.");
            }

            //Contract.Assert(!isSafeOpen && numOfPeople > 0, "Thief stole the money.");

            return new Task(() => { });
        }
    }
}
