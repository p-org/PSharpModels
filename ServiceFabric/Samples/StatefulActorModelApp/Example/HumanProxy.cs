using System;
using System.Threading.Tasks;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.PSharp.Actors.Bridge;

namespace Example
{
  
    public class HumanProxy //: Actor, IHuman
    {
        IHuman obj;
        MachineId id;

        public HumanProxy()
        {
            this.obj = new MyHuman();
            Type mt = typeof(ServiceFabricModel.FabricActorMachine);
            id = ActorModel.Runtime.CreateMachine(mt);
            
            ServiceFabricModel.FabricActorMachine.InitEvent iev = new ServiceFabricModel.
                FabricActorMachine.InitEvent(obj);
            ActorModel.Runtime.SendEvent(id, iev);           
        }

        
        public Task<int> Eat(int a, int b, string s)
        {    
            object[] parameters = new object[] { a, b, s };

            ActorCompletionTask<int> task = new ActorCompletionTask<int>();
            
            ServiceFabricModel.FabricActorMachine.ActorEvent ev = new ServiceFabricModel.
                FabricActorMachine.ActorEvent(typeof(IHuman), "Eat", obj, parameters, task.ActorCompletionMachine);
            ActorModel.Runtime.SendEvent(id, ev);

            return task;
        }
    }
}
