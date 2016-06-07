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
    public class Kitchen : Actor, IKitchen
    {
        protected override Task OnActivateAsync()
        {
            if (!(this.StateManager.ContainsStateAsync("HasSafe").Result))
            {
                this.StateManager.AddStateAsync("PeopleInside", 0);
                this.StateManager.AddStateAsync("HasSafe", false);
            }

            return base.OnActivateAsync();
        }

        public Task PersonEnters()
        {
            int numOfPeople = this.StateManager.GetStateAsync<int>("PeopleInside").Result;
            numOfPeople++;

            this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
            return new Task(() => { });
        }

        public Task PersonExits()
        {
            int numOfPeople = this.StateManager.GetStateAsync<int>("PeopleInside").Result - 1;
            if (numOfPeople < 0)
            {
                numOfPeople = 0;
            }

            this.StateManager.SetStateAsync("PeopleInside", numOfPeople);
            return new Task(() => { });
        }

        public Task<bool> TryEnterRoom()
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
    }
}
