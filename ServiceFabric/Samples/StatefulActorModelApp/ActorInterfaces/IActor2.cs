using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorInterfaces
{
    public interface IActor2 : IActor
    {
        Task<int> GetValue();
        Task SetValue(int val, IActor1 actor1Proxy);
    }
}
