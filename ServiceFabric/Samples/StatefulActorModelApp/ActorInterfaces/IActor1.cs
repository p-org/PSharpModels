using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorInterfaces
{
    public interface IActor1 : IActor
    {
        Task Foo();
        Task Bar();
    }
}
