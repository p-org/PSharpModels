using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace SmartHome.Interfaces
{
    public interface IThief : IActor
    {
        Task Enter(Location location);
    }
}
