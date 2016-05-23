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

using System.Collections.Generic;

using Microsoft.PSharp;
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

        public static IGrainFactory GrainFactory { get; }

        /// <summary>
        /// The P# runtime.
        /// </summary>
        internal static PSharpRuntime Runtime;

        /// <summary>
        /// The proxy factory.
        /// </summary>
        internal static readonly ProxyFactory ProxyFactory;

        /// <summary>
        /// Set of grain ids.
        /// </summary>
        internal static ISet<GrainId> GrainIds;

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
            GrainClient.Runtime = PSharpRuntime.Create();
            GrainClient.ProxyFactory = new ProxyFactory(new HashSet<string> { });
            GrainClient.GrainIds = new HashSet<GrainId>();
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
            GrainClient.Runtime.Assert(config != null,
                "ClientConfiguration object is null.");
            GrainClient.Configuration = config;
        }

        #endregion
    }
}
