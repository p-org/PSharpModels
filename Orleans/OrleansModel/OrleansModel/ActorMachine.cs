using ActorModel;
using Microsoft.PSharp;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ActorModel
{
    public class ActorMachine : Machine
    {
        #region fields
        Machine refMachine;
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
            public object Result;

            public ReturnEvent(object Result)
            {
                this.Result = Result;
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
            refMachine = this;
        }

        private void OnInitEvent()
        {
            try
            {
                var e = this.ReceivedEvent as InitEvent;
                ConstructorInfo sm = typeof(ActorStateManager).GetConstructors().Single();
                var stateManager = Activator.CreateInstance(typeof(ActorStateManager));
                PropertyInfo prop = e.classInstance.GetType().GetProperty("StateManager", BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(e.classInstance, stateManager, null);
                }

                PropertyInfo rProp = e.classInstance.GetType().GetProperty("refMachine", BindingFlags.Public | BindingFlags.Instance);
                if (null != rProp && rProp.CanWrite)
                {
                    Console.WriteLine("setting ref value: " + refMachine);
                    rProp.SetValue(e.classInstance, refMachine, null);
                }

                MethodInfo mo = typeof(ActorBase).GetMethod("OnActivateAsync", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                mo.Invoke(e.classInstance, new object[] { });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(Environment.ExitCode);
            }            
        }

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
        
        public object GetResult()
        {
            //Receive(typeof(ReturnEvent));
            return 6;
        }
        #endregion
    }
}
