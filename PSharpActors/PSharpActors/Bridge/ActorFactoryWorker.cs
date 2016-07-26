//-----------------------------------------------------------------------
// <copyright file="ActorFactoryWorker.cs">
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

namespace Microsoft.PSharp.Actors.Bridge
{
    /// <summary>
    /// The P# actor factory worker machine.
    /// </summary>
    internal abstract class ActorFactoryWorker<ActorIdType> : Machine
    {
        #region static fields

        /// <summary>
        /// Map from actor types to proxy types.
        /// </summary>
        protected static Dictionary<Type, Type> ProxyTypeCache;

        #endregion

        #region fields

        /// <summary>
        /// Map from actor ids to proxy objects.
        /// </summary>
        private Dictionary<ActorIdType, object> ActorProxyMap;

        /// <summary>
        /// The assembly path.
        /// </summary>
        protected string AssemblyPath;

        #endregion

        #region constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ActorFactoryWorker()
        {
            ProxyTypeCache = new Dictionary<Type, Type>();
        }

        #endregion

        #region states

        [Start]
        [OnEntry(nameof(InitOnEntry))]
        private class Init : MachineState { }

        [OnEventDoAction(typeof(ActorFactory.CreateProxyEvent), nameof(CreateProxy))]
        private class Active : MachineState { }

        #endregion

        #region actions

        private void InitOnEntry()
        {
            this.ActorProxyMap = new Dictionary<ActorIdType, object>();
            this.AssemblyPath = (this.ReceivedEvent as ActorFactory.InitEvent).AssemblyPath;

            this.Initialize();

            this.Goto(typeof(Active));
        }

        private void CreateProxy()
        {
            MachineId target = (this.ReceivedEvent as ActorFactory.CreateProxyEvent).Target;
            ActorIdType actorId = (ActorIdType)(this.ReceivedEvent as ActorFactory.CreateProxyEvent).ActorId;
            Type actorType = (this.ReceivedEvent as ActorFactory.CreateProxyEvent).ActorType;

            if (this.ActorProxyMap.ContainsKey(actorId))
            {
                ActorModel.Runtime.Log($"<ActorModelLog> Factory '{this.Id}' contains " +
                    $"actor of type '{actorType.FullName}' with id '{actorId}'. " +
                    $"Requested by {target}.");
                this.Send(target, new ActorFactory.ProxyConstructedEvent(this.ActorProxyMap[actorId]));
                return;
            }

            ActorModel.Runtime.Log($"<ActorModelLog> Factory '{this.Id}' is creating " +
                $"actor proxy of type '{actorType.FullName}'.");

            Type proxyType = null;
            if (ProxyTypeCache.ContainsKey(actorType))
            {
                proxyType = ProxyTypeCache[actorType];
            }
            else
            {
                proxyType = this.GetProxyType(actorType);
                ProxyTypeCache.Add(actorType, proxyType);
            }
            
            object proxy = Activator.CreateInstance(proxyType, actorId);
            
            this.ActorProxyMap.Add(actorId, proxy);

            ActorModel.Runtime.Log($"<ActorModelLog> Factory '{this.Id}' created " +
                $"actor proxy of type '{actorType.FullName}'.");

            this.Send(target, new ActorFactory.ProxyConstructedEvent(proxy));
        }

        #endregion

        #region protected methods

        protected abstract void Initialize();

        protected abstract Type GetProxyType(Type actorType);

        #endregion
    }
}
