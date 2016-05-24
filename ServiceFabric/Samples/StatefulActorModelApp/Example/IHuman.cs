using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public interface IHuman : IActor
    {
        Task<int> Eat(int x, int y, string s);

        Task Foo();
    }
}
