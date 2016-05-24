using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
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

            ServiceFabricModel.FabricActorMachine.InitEvent iev = new ServiceFabricModel.FabricActorMachine.InitEvent(obj);
            rt.SendEvent(id, iev);           
        }

        
        public Task<int> Eat(int a, int b, string s)
        {    
            object[] parameters = new object[] { a, b, s };

            Task<int> returnTask = new Task<int>(() => { return default(int); });
            ServiceFabricModel.FabricActorMachine.ActorEvent ev = new ServiceFabricModel.FabricActorMachine.ActorEvent(typeof(IHuman), "Eat", obj, parameters, returnTask.Id);
            rt.SendEvent(id, ev);

            return returnTask;
            /*
            while (true)
            {
               if (ev.result != null)
                    break;
            }
            */
        }

        public int GetResult(Task<int> t)
        {
            ActorMachine machine = (ActorMachine)((Actor)obj).RefMachine;
            object oResult = machine.GetResult(t);
            return (int)oResult;
        }
    }
}
