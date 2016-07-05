//-----------------------------------------------------------------------
// <copyright file="ActorFactory.cs">
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

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;

namespace Microsoft.ServiceFabric.Actors
{
    /// <summary>
    /// The P# actor factory machine.
    /// </summary>
    internal class ActorFactory : Machine
    {
        #region events

        /// <summary>
        /// The init event.
        /// </summary>
        public class InitEvent : Event
        {
            public string AssemblyPath;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="target">MachineId</param>
            public InitEvent(string assemblyPath)
            {
                this.AssemblyPath = assemblyPath;
            }
        }

        /// <summary>
        /// The create proxy event.
        /// </summary>
        public class CreateProxyEvent : Event
        {
            public MachineId Target;
            public ActorId ActorId;
            public Type ActorType;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="target">MachineId</param>
            /// <param name="actorId">ActorId</param>
            /// <param name="actorType">Type</param>
            public CreateProxyEvent(MachineId target, ActorId actorId, Type actorType)
            {
                this.Target = target;
                this.ActorId = actorId;
                this.ActorType = actorType;
            }
        }

        /// <summary>
        /// The actor invocation event.
        /// </summary>
        public class ProxyConstructedEvent : Event
        {
            public object Proxy;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="proxy">Proxy object</param>
            public ProxyConstructedEvent(object proxy)
            {
                this.Proxy = proxy;
            }
        }

        #endregion

        #region fields

        /// <summary>
        /// Map from types to proxy factory workers.
        /// </summary>
        private Dictionary<Type, MachineId> ProxyFactoryWorkers;

        /// <summary>
        /// The assembly path.
        /// </summary>
        private string AssemblyPath;

        #endregion

        #region states

        [Start]
        [OnEntry(nameof(InitOnEntry))]
        private class Init : MachineState { }

        [OnEventDoAction(typeof(CreateProxyEvent), nameof(CreateProxy))]
        private class Active : MachineState { }

        #endregion

        #region actions

        private void InitOnEntry()
        {
            this.ProxyFactoryWorkers = new Dictionary<Type, MachineId>();
            this.AssemblyPath = (this.ReceivedEvent as InitEvent).AssemblyPath;
            
            this.Goto(typeof(Active));
        }

        private void CreateProxy()
        {
            MachineId target = (this.ReceivedEvent as CreateProxyEvent).Target;
            ActorId actorId = (this.ReceivedEvent as CreateProxyEvent).ActorId;
            Type actorType = (this.ReceivedEvent as CreateProxyEvent).ActorType;

            if (!this.ProxyFactoryWorkers.ContainsKey(actorType))
            {
                MachineId worker = ActorModel.Runtime.CreateMachine(typeof(ActorFactoryWorker),
                    new InitEvent(this.AssemblyPath));
                this.ProxyFactoryWorkers.Add(actorType, worker);
            }

            this.Send(this.ProxyFactoryWorkers[actorType], this.ReceivedEvent);
        }

        #endregion
    }
}
