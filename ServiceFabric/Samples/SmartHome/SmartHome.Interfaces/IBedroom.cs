using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace SmartHome.Interfaces
{
    public interface IBedroom : IActor
    {
        Task PersonEnters();

        Task PersonExits();

        Task AccessSafe();

        Task<bool> TryEnterRoom();
        Task TryToStealMoney();
    }
}
