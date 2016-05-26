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

using Microsoft.PSharp.Actors.Bridge;

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
            public MachineId ActorCompletionMachine;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="methodClass">MethodClass</param>
            /// <param name="methodName">MethodName</param>
            /// <param name="classInstance">ClassInstance</param>
            /// <param name="parameters">Parameters</param>
            /// <param name="tcs">TaskCompletionSource</param>
            public ActorEvent(Type methodClass, string methodName, object classInstance,
                object[] parameters, MachineId actorCompletionMachine)
            {
                this.MethodClass = methodClass;
                this.MethodName = methodName;
                this.ClassInstance = classInstance;
                this.Parameters = parameters;
                this.ActorCompletionMachine = actorCompletionMachine;
            }
        }

        #endregion
        MachineId executorMachine;
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
        private class Init : MachineState { }

        #endregion

        #region actions

        private void OnInitEvent()
        {
            var initEvent = this.ReceivedEvent as InitEvent;

            try
            {
                this.Initialize(initEvent);
                executorMachine = CreateMachine(typeof(ActorExecutorMachine));
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

            //For non-FIFO
            if (Random())               
            {
                //For multiple sends
                if(Random())
                    Send(executorMachine, e);
                Send(executorMachine, e);
            }
            else
            {
                Send(Id, e);
            }
            //MethodInfo mi = e.MethodClass.GetMethod(e.MethodName);

            //ActorModel.Runtime.Log($"<ActorModelLog> Machine '{base.Id}' is invoking '{e.MethodName}'.");

            //try
            //{
            //    object result = mi.Invoke(e.ClassInstance, e.Parameters);
            //    this.Send(e.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(result));
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex);
            //    Environment.Exit(Environment.ExitCode);
            //}
        }

        #endregion
    }
}
