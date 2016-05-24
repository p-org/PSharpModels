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
            public IDictionary<int, object> ResultTaskMap;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="classInstance">ClassInstance</param>
            /// <param name="resultTaskMap">Map of task ids to result tasks</param>
            public InitEvent(object classInstance, IDictionary<int, object> resultTaskMap)
            {
                this.ClassInstance = classInstance;
                this.ResultTaskMap = resultTaskMap;
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

        /// <summary>
        /// Map from task ids to result tasks.
        /// </summary>
        IDictionary<int, object> ResultTaskMap;

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
            var initEvent = this.ReceivedEvent as InitEvent;
            this.ResultTaskMap = initEvent.ResultTaskMap;

            try
            {
                this.Initialize(initEvent);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(Environment.ExitCode);
            }            
        }

        protected abstract void Initialize(InitEvent initEvent);

        private void OnActorEvent()
        {
            var e = (this.ReceivedEvent as ActorEvent);
            MethodInfo mi = e.MethodClass.GetMethod(e.MethodName);
            try
            {
                // TODO: check if we can associate this task with the
                // dummy task returned to the user
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
            this.ResultTaskMap.Add(e.returnForTask, e.Result);
        }

        #endregion
    }
}
