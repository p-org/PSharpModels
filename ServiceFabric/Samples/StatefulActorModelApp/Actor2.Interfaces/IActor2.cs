using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dummy.System.Threading.Tasks;

namespace Actor2.Interfaces
{
    public interface IActor2 : IActor
    {
        Task<int> GetValue();
        Task SetValue(int val);
        int GetResult(Task<int> t);
    }
}
