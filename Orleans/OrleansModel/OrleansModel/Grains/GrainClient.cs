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

        /// <summary>
        /// The P# runtime.
        /// </summary>
        private static PSharpRuntime Runtime;

        /// <summary>
        /// The proxy factory.
        /// </summary>
        private static readonly ProxyFactory<IGrain> ProxyFactory;

        /// <summary>
        /// Map from ids to grains.
        /// </summary>
        internal static Dictionary<IGrain, Object> IdMap;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static GrainClient()
        {
            GrainClient.Runtime = null;
            GrainClient.ProxyFactory = new ProxyFactory<IGrain>();
            GrainClient.IdMap = new Dictionary<IGrain, object>();
        }
    }
}
