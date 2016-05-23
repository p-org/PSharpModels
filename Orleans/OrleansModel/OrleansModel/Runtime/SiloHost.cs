//-----------------------------------------------------------------------
// <copyright file="SiloHost.cs">
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
using System.IO;

using Orleans.Runtime.Configuration;

namespace Orleans.Runtime.Host
{
    /// <summary>
    /// Orleans silo host.
    /// </summary>
    public class SiloHost : IDisposable
    {
        #region fields

        /// <summary>
        /// The name of this silo.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of this silo.
        /// </summary>
        public Silo.SiloType Type { get; set; }

        /// <summary>
        /// Configuration data for the Orleans system.
        /// </summary>
        public ClusterConfiguration Config { get; set; }

        /// <summary>
        /// Deployment id for the cluster this silo is running in.
        /// </summary>
        public string DeploymentId { get; set; }

        /// <summary>
        /// Checks if this silo started successfully
        /// and is currently running.
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Debug flag.
        /// </summary>
        public bool Debug { get; set; }

        #endregion

        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="siloName">Silo name</param>
        public SiloHost(string siloName)
        {
            this.Name = siloName;
            this.Type = Silo.SiloType.Secondary;
            this.IsStarted = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="siloName">Silo name</param>
        /// <param name="config"></param>
        public SiloHost(string siloName, ClusterConfiguration config)
        {
            this.Name = siloName;
            this.Type = Silo.SiloType.Secondary;
            this.IsStarted = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="siloName">Silo name</param>
        /// <param name="configFile"></param>
        public SiloHost(string siloName, FileInfo configFile)
        {
            this.Name = siloName;
            this.Type = Silo.SiloType.Secondary;
            this.IsStarted = false;
        }

        #endregion

        #region methods

        /// <summary>
        /// Initializes this silo.
        /// </summary>
        public void InitializeOrleansSilo()
        {
            
        }

        /// <summary>
        /// Uninitializes this silo.
        /// </summary>
        public void UnInitializeOrleansSilo()
        {

        }

        /// <summary>
        /// Starts this silo.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool StartOrleansSilo(bool catchExceptions = true)
        {
            this.IsStarted = true;
            return true;
        }

        /// <summary>
        /// Stops this silo.
        /// </summary>
        public void StopOrleansSilo()
        {
            this.IsStarted = false;
        }

        /// <summary>
        /// Gracefully shutdowns this silo.
        /// </summary>
        public void ShutdownOrleansSilo()
        {
            this.IsStarted = false;
        }

        /// <summary>
        /// Search for and load the config file for this silo.
        /// </summary>
        public void LoadOrleansConfig()
        {
            this.Config = new ClusterConfiguration();
        }

        /// <summary>
        /// Report an error during silo startup.
        /// </summary>
        /// <param name="exc">Exception</param>
        public void ReportStartupError(Exception exc)
        {

        }

        /// <summary>
        /// Disposes unmanaged resources.
        /// </summary>
        public void Dispose()
        {

        }

        #endregion
    }
}
