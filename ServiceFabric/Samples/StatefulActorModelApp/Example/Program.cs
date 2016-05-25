using System;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var obj = ActorProxy.Create<IHuman>(new ActorId(2), "ABC");
            Task<int> t = obj.Eat(9, 2, "qwer");
            int x = ActorModel.GetResult<int>(t);
            Console.WriteLine(x);

            //var obj = new HumanProxy(PSharpRuntime.Create());
            //Task<int> t = obj.Eat(10, 97, "asdf");
            //int x = ActorModel.GetResult<int>(t);
            //Console.WriteLine(x);

            Console.ReadLine();                        
        }
    }
}
