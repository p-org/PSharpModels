using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor1.interfaces
{
    public interface IActor1 : IActor
    {
        Task Foo();

        TResult GetResult<TResult>(Task<TResult> task);

        void Wait<TResult>(Task<TResult> task);

        void Wait(Task task);
    }
}
