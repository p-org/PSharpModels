//-----------------------------------------------------------------------
// <copyright file="ActorMachine.cs">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// A P# actor machine.
    /// </summary>
    public abstract class ActorMachine : Machine
    {
        #region events

        /// <summary>
        /// The actor initialization event.
        /// </summary>
        public class InitEvent : Event
        {
            public object ClassInstance;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="classInstance">ClassInstance</param>
            public InitEvent(object classInstance)
            {
                this.ClassInstance = classInstance;
            }
        }

        /// <summary>
        /// The actor invocation event.
        /// </summary>
        public class ActorEvent : Event
        {
            public Type MethodClass;
            public string MethodName;
            public object ClassInstance;
            public object[] Parameters;
            public int returnTaskId;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="methodClass">MethodClass</param>
            /// <param name="methodName">MethodName</param>
            /// <param name="classInstance">ClassInstance</param>
            /// <param name="parameters">Parameters</param>
            public ActorEvent(Type methodClass, string methodName, object classInstance, object[] parameters, int returnTaskId)
            {
                this.MethodClass = methodClass;
                this.MethodName = methodName;
                this.ClassInstance = classInstance;
                this.Parameters = parameters;
                this.returnTaskId = returnTaskId;
            }
        }

        /// <summary>
        /// The actor return event.
        /// </summary>
        public class ReturnEvent : Event
        {
            public int returnForTask;
            public object Result;
            
            public ReturnEvent(object Result, int returnForTask)
            {
                this.Result = Result;
                this.returnForTask = returnForTask;
            }
        }

        #endregion

        #region fields
        Dictionary<int, object> resultStore = new Dictionary<int, object>();
        #endregion

        #region states

        [Start]
        [OnEventDoAction(typeof(InitEvent), nameof(OnInitEvent))]
        [OnEventDoAction(typeof(ActorEvent), nameof(OnActorEvent))]
        [OnEventDoAction(typeof(ReturnEvent), nameof(OnReturnEvent))]
        private class Init : MachineState { }

        #endregion

        #region actions

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
            MethodInfo mi = e.MethodClass.GetMethod(e.MethodName);
            try
            {
                object result = mi.Invoke(e.ClassInstance, e.Parameters);
                Send(Id, new ReturnEvent(result, e.returnTaskId));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(Environment.ExitCode);
            }
        }
        
        private void OnReturnEvent()
        {
            var e = ReceivedEvent as ReturnEvent;
            Console.WriteLine("Task completed: " + e.returnForTask);
            resultStore.Add(e.returnForTask, e.Result);
        }

        public object GetResult(Task<int> t)
        {
            if (resultStore.ContainsKey(t.Id))
            {
                Console.WriteLine("returning!!!!!!");
                return resultStore[t.Id];
            }

            this.Receive(typeof(ReturnEvent), retEvent => ((ReturnEvent)retEvent).returnForTask == t.Id);
            return (int)((this.ReceivedEvent as ReturnEvent).Result);
        }

        #endregion
    }
}
