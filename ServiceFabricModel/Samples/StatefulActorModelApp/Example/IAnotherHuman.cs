using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public interface IAnotherHuman : IActor
    {
        Task<int> Play(int x, int y, string s);
    }
}
