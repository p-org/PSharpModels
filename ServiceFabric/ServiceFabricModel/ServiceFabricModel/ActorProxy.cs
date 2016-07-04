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
        internal static ConcurrentDictionary<ActorId, object> IdMap;

        static ActorProxy()
        {
            ActorProxy.ProxyFactory = new ProxyFactory<Actor>(
                new HashSet<string> { "Microsoft.ServiceFabric.Actors" });
            ActorProxy.IdMap = new ConcurrentDictionary<ActorId, object>();

            ActorModel.RegisterCleanUpAction(() =>
            {
                ActorProxy.IdMap.Clear();
            });
        }

        public static TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null,
            string serviceName = null) where TActorInterface : IActor
        {
            if (IdMap.ContainsKey(actorId))
            {
                return (TActorInterface)IdMap[actorId];
            }

            if (ActorModel.Runtime == null)
            {
                throw new InvalidOperationException("The P# runtime has not been initialized.");
            }

            ActorModel.Runtime.Log("<ActorModelLog> Creating actor proxy of type '{0}'.",
                typeof(TActorInterface).FullName);

            string assemblyPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

            Type proxyType = ProxyFactory.GetProxyType(typeof(TActorInterface),
                typeof(FabricActorMachine), assemblyPath);
            var res = (TActorInterface)Activator.CreateInstance(proxyType, actorId);

            ActorModel.Runtime.Log("<ActorModelLog> Created actor proxy of type '{0}'.",
                typeof(TActorInterface).FullName);
            
            return res;
        }
    }
}
