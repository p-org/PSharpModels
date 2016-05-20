using ServiceFabricModel;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors.Bridge;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorProxy
    {
        private static PSharpRuntime runtime;
        private static readonly ProxyFactory<ActorId> ProxyFactory;
        private static Dictionary<ActorId, Object> IdMap;

        static ActorProxy()
        {
            ProxyFactory = new ProxyFactory<ActorId>(new HashSet<string> { "Microsoft.ServiceFabric.Actors" });
            runtime = null;
            IdMap = new Dictionary<ActorId, object>();
    }

        public static TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null, string serviceName = null) where TActorInterface : IActor
        {
            if (IdMap.ContainsKey(actorId))
                return (TActorInterface)IdMap[actorId];

            if (runtime == null)
                runtime = PSharpRuntime.Create();

            Type proxyType = ProxyFactory.GetProxyType(typeof(TActorInterface), typeof(FabricActorMachine));
            var res = (TActorInterface)Activator.CreateInstance(proxyType, runtime);
            IdMap.Add(actorId, res);
            return res;
        }

    }
}
