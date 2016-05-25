using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors.Bridge;

namespace Example
{
  
    public class HumanProxy //: Actor, IHuman
    {
        IHuman obj;
        MachineId id;
        PSharpRuntime rt;

        public HumanProxy(PSharpRuntime runtime)
        {
            this.obj = new MyHuman();
            rt = runtime;
            rt = PSharpRuntime.Create();
            Type mt = typeof(ServiceFabricModel.FabricActorMachine);
            id = rt.CreateMachine(mt);
            
            ServiceFabricModel.FabricActorMachine.InitEvent iev = new ServiceFabricModel.
                FabricActorMachine.InitEvent(obj);
            rt.SendEvent(id, iev);           
        }

        
        public Task<int> Eat(int a, int b, string s)
        {    
            object[] parameters = new object[] { a, b, s };

            ActorCompletionTask<int> task = new ActorCompletionTask<int>(rt);
            
            ServiceFabricModel.FabricActorMachine.ActorEvent ev = new ServiceFabricModel.
                FabricActorMachine.ActorEvent(typeof(IHuman), "Eat", obj, parameters, task.ActorCompletionMachine);
            rt.SendEvent(id, ev);

            return task;
        }

        public Task Foo()
        {
            Console.WriteLine("HI");

            object[] parameters = new object[] { };

            ActorCompletionTask<int> task = new ActorCompletionTask<int>(rt);

            ServiceFabricModel.FabricActorMachine.ActorEvent ev = new ServiceFabricModel.
                FabricActorMachine.ActorEvent(typeof(IHuman), "Foo", obj, parameters, task.ActorCompletionMachine);
            rt.SendEvent(id, ev);

            return task;
        }

        public TResult GetResult<TResult>(Task<TResult> task)
        {
            return ((ActorCompletionTask<TResult>)task).Result;
        }

        public void Wait(Task task)
        {
            ((ActorCompletionTask)task).Wait();
        }
    }
}
