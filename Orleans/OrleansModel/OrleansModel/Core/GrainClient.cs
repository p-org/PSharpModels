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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.PSharp.Actors.Bridge;

using Orleans.Runtime.Configuration;
using OrleansModel;

namespace Orleans
{
    /// <summary>
    /// Client runtime for connecting to the Orleans system.
    /// </summary>
    public static class GrainClient
    {
        #region fields

        public static IGrainFactory GrainFactory { get; private set; }

        /// <summary>
        /// The Orleans grain runtime.
        /// </summary>
        internal static GrainRuntime Runtime;

        /// <summary>
        /// The proxy factory machine.
        /// </summary>
        internal static MachineId ProxyFactory;

        /// <summary>
        /// Set of grain ids.
        /// </summary>
        internal static ConcurrentBag<GrainId> GrainIds;

        /// <summary>
        /// The client configuration.
        /// </summary>
        internal static ClientConfiguration Configuration { get; private set; }

        #endregion

        #region constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static GrainClient()
        {
            GrainClient.GrainFactory = new GrainFactory();
            GrainClient.Runtime = new GrainRuntime(GrainClient.GrainFactory);
            GrainClient.GrainIds = new ConcurrentBag<GrainId>();

            string assemblyPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            ProxyFactory = ActorModel.Runtime.CreateMachine(typeof(OrleansGrainFactory),
                new ActorFactory.InitEvent(assemblyPath));

            ActorModel.RegisterCleanUpAction(() =>
            {
                GrainClient.GrainIds = new ConcurrentBag<GrainId>();
                ProxyFactory = ActorModel.Runtime.CreateMachine(typeof(OrleansGrainFactory),
                    new ActorFactory.InitEvent(assemblyPath));
            });
        }

        #endregion

        #region methods

        /// <summary>
        /// Initializes the client runtime from the provided
        /// client configuration object. If the configuration
        /// object is null, the initialization fails.
        /// </summary>
        /// <param name="config">ClientConfiguration</param>
        public static void Initialize(ClientConfiguration config)
        {
            ActorModel.Runtime.Assert(config != null,
                "ClientConfiguration object is null.");
            GrainClient.Configuration = config;
        }

        #endregion
    }
}
