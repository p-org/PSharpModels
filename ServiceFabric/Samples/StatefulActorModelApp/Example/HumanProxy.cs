using Microsoft.PSharp;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Threading.Tasks;

namespace Example
{
  
    public class HumanProxy //: IHuman
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
            ActorId aid = new ActorId(5);
            ServiceFabricModel.FabricActorMachine.InitEvent iev = new ServiceFabricModel.FabricActorMachine.InitEvent(obj);
            rt.SendEvent(id, iev);           
        }

        
        public Task<int> Eat(int a, int b, string s)
        {    
            object[] parameters = new object[] { a, b, s };


            ServiceFabricModel.FabricActorMachine.ActorEvent ev = new ServiceFabricModel.FabricActorMachine.ActorEvent(typeof(IHuman), "Eat", obj, parameters);
            rt.SendEvent(id, ev);

            return new Task<int>(() => { return default(int); });
            /*
            while (true)
            {
               if (ev.result != null)
                    break;
            }
            */
        }       
    }
}
