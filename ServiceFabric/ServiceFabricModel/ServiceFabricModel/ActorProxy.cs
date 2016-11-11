//-----------------------------------------------------------------------
// <copyright file="ActorProxy.cs">
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
using System.IO;
using System.Reflection;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.PSharp.Actors.Bridge;

using ServiceFabricModel;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorProxy
    {
        /// <summary>
        /// The proxy factory machine.
        /// </summary>
        private static MachineId ProxyFactory;

        static ActorProxy()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            ProxyFactory = ActorModel.Runtime.CreateMachine(typeof(FabricActorFactory),
                new ActorFactory.InitEvent(assemblyPath));
            
            ActorModel.RegisterCleanUpAction(() =>
            {
                ProxyFactory = ActorModel.Runtime.CreateMachine(typeof(FabricActorFactory),
                    new ActorFactory.InitEvent(assemblyPath));
            });
        }

        public static TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null,
            string serviceName = null) where TActorInterface : IActor
        {
            if (ActorModel.Runtime == null)
            {
                throw new InvalidOperationException("The P# runtime has not been initialized.");
            }
            
            MachineId mid = ActorModel.Runtime.GetCurrentMachineId();

            ActorModel.Runtime.Log($"<ActorModelLog> Machine '{mid.Name}' is " +
                $"waiting to get or construct proxy with id '{actorId.Id}'.");

            ActorModel.Runtime.SendEvent(ProxyFactory, new ActorFactory.CreateProxyEvent(
                mid, actorId, typeof(TActorInterface)));

            ActorFactory.ProxyConstructedEvent receivedEvent = ActorModel.Runtime.Receive(
                typeof(ActorFactory.ProxyConstructedEvent)) as ActorFactory.ProxyConstructedEvent;
            
            TActorInterface proxy = (TActorInterface)receivedEvent.Proxy;

            ActorModel.Runtime.Log($"<ActorModelLog> Machine '{mid.Name}' received " +
                $"proxy with id '{actorId.Id}'.");

            return proxy;
        }

        public static TActorInterface Create<TActorInterface>(ActorId actorId, Uri applicationName = null) where TActorInterface : IActor
        {
            if (ActorModel.Runtime == null)
            {
                throw new InvalidOperationException("The P# runtime has not been initialized.");
            }

            MachineId mid = ActorModel.Runtime.GetCurrentMachineId();

            ActorModel.Runtime.Log($"<ActorModelLog> Machine '{mid.Name}' is " +
                $"waiting to get or construct proxy with id '{actorId.Id}'.");

            ActorModel.Runtime.SendEvent(ProxyFactory, new ActorFactory.CreateProxyEvent(
                mid, actorId, typeof(TActorInterface)));

            ActorFactory.ProxyConstructedEvent receivedEvent = ActorModel.Runtime.Receive(
                typeof(ActorFactory.ProxyConstructedEvent)) as ActorFactory.ProxyConstructedEvent;

            TActorInterface proxy = (TActorInterface)receivedEvent.Proxy;

            ActorModel.Runtime.Log($"<ActorModelLog> Machine '{mid.Name}' received " +
                $"proxy with id '{actorId.Id}'.");

            return proxy;
        }
    }
}
