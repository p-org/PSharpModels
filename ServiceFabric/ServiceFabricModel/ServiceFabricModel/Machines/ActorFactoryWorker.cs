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

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.PSharp.Actors.Bridge;

using Microsoft.ServiceFabric.Actors.Runtime;
using ServiceFabricModel;

namespace Microsoft.ServiceFabric.Actors
{
    /// <summary>
    /// The P# actor factory worker machine.
    /// </summary>
    internal class ActorFactoryWorker : Machine
    {
        #region static fields

        /// <summary>
        /// Map from actor types to proxy types.
        /// </summary>
        private static Dictionary<Type, Type> ProxyTypeCache;

        #endregion

        #region fields

        /// <summary>
        /// The actor proxy factory.
        /// </summary>
        private ProxyFactory<Actor> ProxyFactory;

        /// <summary>
        /// Map from actor ids to proxy objects.
        /// </summary>
        private Dictionary<ActorId, object> ActorProxyMap;

        /// <summary>
        /// The assembly path.
        /// </summary>
        private string AssemblyPath;

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
            this.ProxyFactory = new ProxyFactory<Actor>(
                new HashSet<string> { "Microsoft.ServiceFabric.Actors" });
            this.ActorProxyMap = new Dictionary<ActorId, object>();
            this.AssemblyPath = (this.ReceivedEvent as ActorFactory.InitEvent).AssemblyPath;
            
            this.Goto(typeof(Active));
        }

        private void CreateProxy()
        {
            MachineId target = (this.ReceivedEvent as ActorFactory.CreateProxyEvent).Target;
            ActorId actorId = (this.ReceivedEvent as ActorFactory.CreateProxyEvent).ActorId;
            Type actorType = (this.ReceivedEvent as ActorFactory.CreateProxyEvent).ActorType;

            if (this.ActorProxyMap.ContainsKey(actorId))
            {
                ActorModel.Runtime.Log($"<ActorModelLog> Factory '{this.Id}' contains " +
                    $"actor of type '{actorType.FullName}' with id '{actorId.Id}'. " +
                    $"Requested by {target}.");
                this.Send(target, new ActorFactory.ProxyConstructedEvent(this.ActorProxyMap[actorId]));
                return;
            }

            ActorModel.Runtime.Log($"<ActorModelLog> Factory '{this.Id}' is creating " +
                $"actor proxy of type '{actorType.FullName}'.");

            Type proxyType = null;

            Console.WriteLine($"ProxyTypeCache contains {ProxyTypeCache.Count} items.");
            foreach (var x in ProxyTypeCache)
            {
                Console.WriteLine(x.Key + " :: " + x.Value);
            }

            if (ProxyTypeCache.ContainsKey(actorType))
            {
                proxyType = ProxyTypeCache[actorType];
            }
            else
            {
                proxyType = ProxyFactory.GetProxyType(actorType,
                    typeof(FabricActorMachine), this.AssemblyPath);
                ProxyTypeCache.Add(actorType, proxyType);
            }
            
            object proxy = Activator.CreateInstance(proxyType);
            
            this.ActorProxyMap.Add(actorId, proxy);

            ActorModel.Runtime.Log($"<ActorModelLog> Factory '{this.Id}' created " +
                $"actor proxy of type '{actorType.FullName}'.");

            this.Send(target, new ActorFactory.ProxyConstructedEvent(proxy));
        }

        #endregion
    }
}
