//-----------------------------------------------------------------------
// <copyright file="GrainFactory.cs">
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
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.PSharp.Actors.Bridge;

using Orleans;
using Orleans.Runtime;

namespace OrleansModel
{
    /// <summary>
    /// Model implementation of a grain factory.
    /// </summary>
    internal class GrainFactory : IGrainFactory
    {
        #region methods

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithGuidKey
        {
            return this.GetOrCreateGrain<TGrainInterface>(primaryKey);
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithIntegerKey
        {
            Guid key = GrainId.CreateGuid(primaryKey);
            return this.GetOrCreateGrain<TGrainInterface>(key);
        }

        public TGrainInterface GetGrain<TGrainInterface>(string primaryKey,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithStringKey
        {
            Guid key = GrainId.CreateGuid(primaryKey);
            return this.GetOrCreateGrain<TGrainInterface>(key);
        }

        public TGrainInterface GetGrain<TGrainInterface>(Guid primaryKey, string keyExtension,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithGuidCompoundKey
        {
            return this.GetOrCreateGrain<TGrainInterface>(primaryKey);
        }

        public TGrainInterface GetGrain<TGrainInterface>(long primaryKey, string keyExtension,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithIntegerCompoundKey
        {
            Guid key = GrainId.CreateGuid(primaryKey);
            return this.GetOrCreateGrain<TGrainInterface>(key);
        }

        /// <summary>
        /// Gets or creates a grain.
        /// </summary>
        /// <typeparam name="TGrainInterface">TGrainInterface</typeparam>
        /// <param name="primaryKey">PrimaryKey</param>
        /// <returns>TGrainInterface</returns>
        private TGrainInterface GetOrCreateGrain<TGrainInterface>(Guid primaryKey)
        {
            if (ActorModel.Runtime == null)
            {
                throw new InvalidOperationException("The P# runtime has not been initialized.");
            }

            MachineId mid = ActorModel.Runtime.GetCurrentMachine();
            
            ActorModel.Runtime.Log($"<ActorModelLog> Machine '{mid.Name}' is " +
                $"waiting to get or construct proxy with id '{primaryKey}'.");
            
            ActorModel.Runtime.SendEvent(GrainClient.ProxyFactory, new ActorFactory.CreateProxyEvent(
                mid, primaryKey, typeof(TGrainInterface)));

            ActorFactory.ProxyConstructedEvent receivedEvent = ActorModel.Runtime.Receive(
                typeof(ActorFactory.ProxyConstructedEvent)) as ActorFactory.ProxyConstructedEvent;

            TGrainInterface proxy = (TGrainInterface)receivedEvent.Proxy;

            ActorModel.Runtime.Log($"<ActorModelLog> Machine '{mid.Name}' received " +
                $"proxy with id '{primaryKey}'.");

            return proxy;
        }

        #endregion
    }
}