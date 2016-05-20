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

namespace Orleans
{
    /// <summary>
    /// Model implementation of a grain factory.
    /// </summary>
    internal class GrainFactory : IGrainFactory
    {
        TGrainInterface IGrainFactory.GetGrain<TGrainInterface>(Guid primaryKey,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithGuidKey
        {

        }

        TGrainInterface IGrainFactory.GetGrain<TGrainInterface>(long primaryKey,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithIntegerKey
        {
            if (GrainClient.IdMap.ContainsKey())
        }

        public static TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null, string serviceName = null) where TActorInterface : IActor
        {
            if (IdMap.ContainsKey(actorId))
                return (TActorInterface)IdMap[actorId];

            if (runtime == null)
                runtime = PSharpRuntime.Create();

            Type proxyType = proxies.GetProxyType(typeof(TActorInterface), actorId);
            var res = (TActorInterface)Activator.CreateInstance(proxyType, runtime);
            IdMap.Add(actorId, res);
            return res;
        }

        TGrainInterface IGrainFactory.GetGrain<TGrainInterface>(string primaryKey,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithStringKey
        {

        }

        TGrainInterface IGrainFactory.GetGrain<TGrainInterface>(Guid primaryKey, string keyExtension,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithGuidCompoundKey
        {

        }

        TGrainInterface IGrainFactory.GetGrain<TGrainInterface>(long primaryKey, string keyExtension,
            string grainClassNamePrefix = null)
            where TGrainInterface : IGrainWithIntegerCompoundKey
        {

        }
    }
}