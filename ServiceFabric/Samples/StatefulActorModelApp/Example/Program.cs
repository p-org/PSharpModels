using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PSharp;
using Microsoft.PSharp.Utilities;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var obj = ActorProxy.Create<IHuman>(new ActorId(2), "ABC");
            Task<int> t =  obj.Eat(9, 2, "qwer");
            Thread.Sleep(3000);
            Console.WriteLine("DONE sleeping " + t.Id);
            int x = obj.GetResult(t);
            Console.WriteLine(x);
            //Console.WriteLine(obj.Eat(5, 20, "qwer"));

            /*     
            var obj = new HumanProxy(PSharpRuntime.Create());
            Task<int> t = obj.Eat(10, 97, "asdf");
            Thread.Sleep(10000);
            int x = obj.GetResult(t);
            Console.WriteLine(x);
            */

            Console.ReadLine();                        
        }
    }
}
