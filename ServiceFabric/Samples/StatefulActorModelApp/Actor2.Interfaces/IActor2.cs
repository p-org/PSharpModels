using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor2.Interfaces
{
    public interface IActor2 : IActor
    {
        Task<int> GetValue();
        Task SetValue(int val);

        TResult GetResult<TResult>(Task<TResult> task);

        void Wait<TResult>(Task<TResult> task);

        void Wait(Task task);
    }
}
