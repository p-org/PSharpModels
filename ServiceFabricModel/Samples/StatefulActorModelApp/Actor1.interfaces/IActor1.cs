using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dummy.System.Threading.Tasks;

namespace Actor1.interfaces
{
    public interface IActor1 : IActor
    {
        Task Foo();
    }
}
