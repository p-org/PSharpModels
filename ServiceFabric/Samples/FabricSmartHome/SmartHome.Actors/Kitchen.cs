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
    internal class Kitchen : Actor, IKitchen
    {
        protected override async Task OnActivateAsync()
        {
            if (!(await this.StateManager.ContainsStateAsync("HasSafe")))
            {
                await this.StateManager.AddStateAsync("PeopleInside", 0);
                await this.StateManager.AddStateAsync("HasSafe", false);
            }

            await base.OnActivateAsync();
        }

        async Task IKitchen.PersonEnters()
        {
            int numOfPeople = await this.StateManager.GetStateAsync<int>("PeopleInside");
            numOfPeople++;

            await this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
        }

        async Task IKitchen.PersonExits()
        {
            int numOfPeople = await this.StateManager.GetStateAsync<int>("PeopleInside") - 1;
            if (numOfPeople < 0)
            {
                numOfPeople = 0;
            }

            await this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
        }

        async Task<bool> IKitchen.TryEnterRoom()
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
    }
}
