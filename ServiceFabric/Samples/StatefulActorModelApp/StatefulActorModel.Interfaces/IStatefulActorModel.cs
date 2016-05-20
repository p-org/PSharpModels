using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dummy.System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace StatefulActorModel.Interfaces
{
    public interface IStatefulActorModel : IActor
    {
        Task<int> GetCountAsync();

        Task SetCountAsync(int count);
    }
}
