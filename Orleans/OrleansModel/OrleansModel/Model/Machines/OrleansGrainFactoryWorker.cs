//-----------------------------------------------------------------------
// <copyright file="OrleansGrainFactoryWorker.cs">
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

using Microsoft.PSharp.Actors.Bridge;

using Orleans;
using Orleans.Runtime;

namespace OrleansModel
{
    /// <summary>
    /// The P# fabric actor factory worker machine.
    /// </summary>
    internal class OrleansGrainFactoryWorker : ActorFactoryWorker
    {
        #region fields

        /// <summary>
        /// The actor proxy factory.
        /// </summary>
        private ProxyFactory<Grain> ProxyFactory;

        #endregion

        #region protected methods

        protected override void Initialize()
        {
            this.ProxyFactory = new ProxyFactory<Grain>(
                new HashSet<string> { });
            this.ProxyFactory.RegisterIgnoredInterfaceTypes(new HashSet<Type>
            {
                typeof(IAddressable),
                typeof(IStatefulGrain)
            });
        }

        protected override Type GetProxyType(Type actorType)
        {
            return this.ProxyFactory.GetProxyType(actorType,
                typeof(OrleansGrainMachine), base.AssemblyPath);
        }

        #endregion
    }
}
