using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            Action<object> setResultAction = new Action<object>(task =>
            {
                tcs.SetResult(((Task<int>)task).Result);
            });
            
            ServiceFabricModel.FabricActorMachine.ActorEvent ev = new ServiceFabricModel.
                FabricActorMachine.ActorEvent(typeof(IHuman), "Eat", obj, parameters, setResultAction);
            rt.SendEvent(id, ev);

            return tcs.Task;
        }
    }
}
