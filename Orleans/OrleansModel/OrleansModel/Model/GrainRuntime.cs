//-----------------------------------------------------------------------
// <copyright file="GrainRuntime.cs">
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

using Microsoft.PSharp;

using Orleans;
using Orleans.Runtime;

namespace OrleansModel
{
    /// <summary>
    /// The Orleans grain runtime.
    /// </summary>
    internal class GrainRuntime : IGrainRuntime
    {
        /// <summary>
        /// The P# runtime.
        /// </summary>
        internal PSharpRuntime PSharpRuntime;

        /// <summary>
        /// The grain factory.
        /// </summary>
        public IGrainFactory GrainFactory { get; private set; }

        /// <summary>
        /// The service provider.
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="grainFactory">IGrainFactory</param>
        public GrainRuntime(IGrainFactory grainFactory)
        {
            this.PSharpRuntime = PSharpRuntime.Create();
            this.GrainFactory = grainFactory;
        }
    }
}