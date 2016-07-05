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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.PSharp.Actors.Bridge;
using Microsoft.ServiceFabric.Actors.Runtime;

using ServiceFabricModel;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorProxy
    {
        private static ProxyFactory<Actor> ProxyFactory;
        internal static ConcurrentBag<ActorId> ActorIds;

        static ActorProxy()
        {
            ActorProxy.ProxyFactory = new ProxyFactory<Actor>(
                new HashSet<string> { "Microsoft.ServiceFabric.Actors" });
            ActorProxy.ActorIds = new ConcurrentBag<ActorId>();

            ActorModel.RegisterCleanUpAction(() =>
            {
                ActorProxy.ActorIds = new ConcurrentBag<ActorId>();
            });
        }

        public static TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null,
            string serviceName = null) where TActorInterface : IActor
        {
            Console.WriteLine("Creating actor id of " + typeof(TActorInterface));
            Console.WriteLine(actorId);

            var id = ActorProxy.ActorIds.SingleOrDefault(val => val.Equals(actorId));
            if (id != null)
            {
                ActorModel.Runtime.Log("<ActorModelLog> Found actor of type " +
                    $"'{typeof(TActorInterface).FullName}' with id '{actorId.Id}'.");
                return (TActorInterface)id.Actor;
            }

            Console.WriteLine("Not found");

            if (ActorModel.Runtime == null)
            {
                throw new InvalidOperationException("The P# runtime has not been initialized.");
            }

            ActorModel.Runtime.Log("<ActorModelLog> Creating actor proxy of type '{0}'.",
                typeof(TActorInterface).FullName);

            string assemblyPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            Type proxyType = ProxyFactory.GetProxyType(typeof(TActorInterface),
                typeof(FabricActorMachine), assemblyPath);

            Action<object> registerActorCallback = new Action<object>(proxy =>
            {
                actorId.Actor = proxy;
                ActorProxy.ActorIds.Add(actorId);
            });

            var res = (TActorInterface)Activator.CreateInstance(proxyType, registerActorCallback);

            ActorModel.Runtime.Log("<ActorModelLog> Created actor proxy of type '{0}'.",
                typeof(TActorInterface).FullName);
            
            return res;
        }
    }
}
