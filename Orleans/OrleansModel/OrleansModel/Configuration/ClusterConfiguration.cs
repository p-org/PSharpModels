//-----------------------------------------------------------------------
// <copyright file="ClusterConfiguration.cs">
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

using Orleans.Storage;

namespace Orleans.Runtime.Configuration
{
    /// <summary>
    /// Orleans cluster configuration parameters.
    /// </summary>
    public class ClusterConfiguration
    {
        #region methods

        /// <summary>
        /// Returns a prepopulated ClusterConfiguration object for a primary local silo.
        /// </summary>
        /// <param name="siloPort">TCP port for silo to silo communication</param>
        /// <param name="gatewayPort">Client gateway TCP port</param>
        /// <returns>ClusterConfiguration</returns>
        public static ClusterConfiguration LocalhostPrimarySilo(int siloPort = 22222, int gatewayPort = 40000)
        {
            var config = new ClusterConfiguration();
            return config;
        }

        /// <summary>
        /// Adds a storage provider of type <see cref="MemoryStorage"/>
        /// </summary>
        /// <param name="config">The cluster configuration object to add provider to.</param>
        /// <param name="providerName">The provider name.</param>
        /// <param name="numStorageGrains">The number of storage grains to use.</param>
        public void AddMemoryStorageProvider(string providerName = "MemoryStore",
            int numStorageGrains = MemoryStorage.NumStorageGrainsDefaultValue)
        {

        }

        #endregion
    }
}
