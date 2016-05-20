//-----------------------------------------------------------------------
// <copyright file="GrainClient.cs">
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

using OrleansModel;

namespace Orleans
{
    /// <summary>
    /// Client runtime for connecting to the Orleans system.
    /// </summary>
    public static class GrainClient
    {
        public static IGrainFactory GrainFactory { get; }

        //private static readonly ProxyContainer proxies = new ProxyContainer();

        //private static PSharpRuntime runtime = null;

        //private static Dictionary<ActorId, Object> IdMap = new Dictionary<ActorId, object>();

        //public static TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null, string serviceName = null) where TActorInterface : IActor
        //{
        //    if (IdMap.ContainsKey(actorId))
        //        return (TActorInterface)IdMap[actorId];

        //    if (runtime == null)
        //        runtime = PSharpRuntime.Create();

        //    Type proxyType = proxies.GetProxyType(typeof(TActorInterface), actorId);
        //    var res = (TActorInterface)Activator.CreateInstance(proxyType, runtime);
        //    IdMap.Add(actorId, res);
        //    return res;
        //}
    }
}
