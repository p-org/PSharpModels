using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Microsoft.PSharp;
using System.Threading.Tasks;

namespace Microsoft.PSharp.Actors
{
    public abstract class ActorMachine : Machine
    {
        #region fields
        protected Machine RefMachine;
        #endregion

        #region events
        public class ActorEvent : Event
        {
            public Type methodClass;
            public string methodName;
            public object classInstance;
            public object[] parameters;
            public object result;

            public ActorEvent(Type methodClass, string methodName, object classInstance, object[] parameters)
            {
                this.methodClass = methodClass;
                this.methodName = methodName;
                this.classInstance = classInstance;
                this.parameters = parameters;
            }
        }

        public class InitEvent : Event
        {
            public object classInstance;
            public InitEvent(object classInstance)
            {
                this.classInstance = classInstance;
            }
        }

        public class ReturnEvent : Event
        {
            public Task<int> returnForTask;
            public object Result;

            public ReturnEvent(object Result, Task<int> returnForTask)
            {
                this.Result = Result;
                this.returnForTask = returnForTask;
            }
        }
        #endregion

        #region fields
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(assignRef))]
        [OnEventDoAction(typeof(InitEvent), nameof(OnInitEvent))]
        [OnEventDoAction(typeof(ActorEvent), nameof(OnActorEvent))]
        private class Init : MachineState { }
        #endregion

        #region actions
        void assignRef()
        {
            this.RefMachine = this;
        }

        private void OnInitEvent()
        {
            try
            {
                this.Initialize();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(Environment.ExitCode);
            }            
        }

        protected abstract void Initialize();

        private void OnActorEvent()
        {
            var e = (this.ReceivedEvent as ActorEvent);
            MethodInfo mi = e.methodClass.GetMethod(e.methodName);
            try
            {
                e.result = mi.Invoke(e.classInstance, e.parameters);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(Environment.ExitCode);
            }
        }
        
        public object GetResult(Task<int> t)
        {
            this.Receive(typeof(ReturnEvent), retEvent => ((ReturnEvent)retEvent).returnForTask.Id == t.Id);
            return (int)((this.ReceivedEvent as ReturnEvent).Result);
        }
        #endregion
    }
}
