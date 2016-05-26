using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sender.Interfaces
{
    public interface ISender : IActor
    {
        Task DoSomething(int numberOfItems);
    }
}
