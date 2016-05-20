using OrleansModel;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.PSharp;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorProxy
    {
        private static readonly ProxyContainer proxies = new ProxyContainer();

        private static PSharpRuntime runtime = null;

        private static Dictionary<ActorId, Object> IdMap = new Dictionary<ActorId, object>();

        public static TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null, string serviceName = null) where TActorInterface : IActor
        {
            if (IdMap.ContainsKey(actorId))
                return (TActorInterface)IdMap[actorId];

            if (runtime == null)
                runtime = PSharpRuntime.Create();

            Type proxyType = proxies.GetProxyType(typeof(TActorInterface), actorId);
            var res = (TActorInterface)Activator.CreateInstance(proxyType, runtime);
            IdMap.Add(actorId, res);
            return res;
        }
    }
}
