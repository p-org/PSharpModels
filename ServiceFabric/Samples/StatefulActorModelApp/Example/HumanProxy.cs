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

        Dictionary<int, object> ResultTaskMap;

        public HumanProxy(PSharpRuntime runtime)
        {
            this.ResultTaskMap = new Dictionary<int, object>();
            this.obj = new MyHuman();
            rt = runtime;
            rt = PSharpRuntime.Create();
            Type mt = typeof(ServiceFabricModel.FabricActorMachine);
            id = rt.CreateMachine(mt);


            ServiceFabricModel.FabricActorMachine.InitEvent iev = new ServiceFabricModel.FabricActorMachine.InitEvent(obj, this.ResultTaskMap);
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
            if (this.ResultTaskMap.ContainsKey(t.Id))
            {
                return ((Task<int>)this.ResultTaskMap[t.Id]).Result;
            }

            Event returnEvent = rt.Receive(id, typeof(ActorMachine.ReturnEvent),
                retEvent => ((ActorMachine.ReturnEvent)retEvent).returnForTask == t.Id);
            var result = ((Task<int>)((ActorMachine.ReturnEvent)returnEvent).Result).Result;
            return result;
        }
    }
}
