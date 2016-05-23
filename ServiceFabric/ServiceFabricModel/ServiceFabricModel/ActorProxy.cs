using ServiceFabricModel;
using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors.Bridge;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorProxy
    {
        private static PSharpRuntime runtime;
        private static readonly ProxyFactory ProxyFactory;
        private static Dictionary<ActorId, Object> IdMap;

        static ActorProxy()
        {
            ProxyFactory = new ProxyFactory(new HashSet<string> { "Microsoft.ServiceFabric.Actors" });
            runtime = null;
            IdMap = new Dictionary<ActorId, object>();
    }

        public static TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null,
            string serviceName = null) where TActorInterface : IActor
        {
            if (IdMap.ContainsKey(actorId))
                return (TActorInterface)IdMap[actorId];

            if (runtime == null)
                runtime = PSharpRuntime.Create();

            string assemblyPath = Assembly.GetEntryAssembly().Location + "\\..\\..\\..\\..";

            Type proxyType = ProxyFactory.GetProxyType(typeof(TActorInterface),
                typeof(FabricActorMachine), assemblyPath);
            var res = (TActorInterface)Activator.CreateInstance(proxyType, runtime);
            IdMap.Add(actorId, res);
            return res;
        }
    }
}
