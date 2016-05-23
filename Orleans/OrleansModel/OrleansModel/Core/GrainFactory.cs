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
using System.Linq;

using OrleansModel;

namespace Orleans
{
    /// <summary>
    /// Model implementation of a grain factory.
    /// </summary>
    internal class GrainFactory : IGrainFactory
    {
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
            var id = GrainClient.GrainIds.SingleOrDefault(val => val.PrimaryKey.Equals(primaryKey));
            if (id != null)
            {
                return (TGrainInterface)id.Grain;
            }

            Console.WriteLine("Creating grain: " + typeof(TGrainInterface));

            Type proxyType = GrainClient.ProxyFactory.GetProxyType(
                typeof(TGrainInterface), typeof(OrleansActorMachine));
            var grain = (TGrainInterface)Activator.CreateInstance(
                proxyType, GrainClient.Runtime);

            GrainId newId = new GrainId(primaryKey, (IGrain)grain);
            return grain;
        }
    }
}