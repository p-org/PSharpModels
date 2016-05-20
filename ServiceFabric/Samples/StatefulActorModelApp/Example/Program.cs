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
            
            var obj = ActorProxy.Create<IHuman>(ActorId.NewId(), " ");
            Task<int> t =  obj.Eat(9, 2, "qwer");
            Console.WriteLine(obj.Eat(5, 20, "qwer"));

            /*       
            var obj = new HumanProxy();
            Task<int> t = obj.Eat(10, 97, "asdf");
            Console.WriteLine("task id: " + t.taskId);
            Task<int> u = obj.Eat(1, 7, "lkj");
            Console.WriteLine(u.taskId);
            /*var obj1 = new HumanProxy();
            obj1.Eat(60, 9, "asdf");
            */
            Console.ReadLine();                        
        }
    }
}
