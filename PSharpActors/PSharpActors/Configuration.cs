//-----------------------------------------------------------------------
// <copyright file="Configuration.cs">
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
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors.Bridge;

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// The P# actor model configuration.
    /// </summary>
    public class Configuration
    {
        #region fields
        
        internal bool UseFirstInFirstOutOrder;

        internal bool DoMultipleSends;

        internal bool DoLifetimeManagement;

        internal bool PerformSerialization;

        #endregion

        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        private Configuration()
        {
            this.PerformSerialization = true;
            this.DoMultipleSends = true;
            this.UseFirstInFirstOutOrder = false;
            this.DoLifetimeManagement = true;
        }

        #endregion

        #region methods

        /// <summary>
        /// Creates the default P# actor model configuration.
        /// </summary>
        /// <returns>Configuration</returns>
        internal static Configuration Default()
        {
            return new Configuration();
        }

        /// <summary>
        /// Creates a P# actor model configuration
        /// with the specified options.
        /// </summary>
        /// <param name="performSerialization">Perform serialization</param>
        /// <param name="doMultipleSends">Do multiple sends</param>
        /// <param name="useFirstInFirstOutOrder">Use FIFO order</param>
        /// <param name="doLifetimeManagement">Do lifetime management</param>
        /// <returns>Configuration</returns>
        public static Configuration Create(bool performSerialization, bool doMultipleSends,
            bool useFirstInFirstOutOrder, bool doLifetimeManagement)
        {
            var config = Configuration.Default();

            config.PerformSerialization = performSerialization;
            config.DoMultipleSends = doMultipleSends;
            config.UseFirstInFirstOutOrder = useFirstInFirstOutOrder;
            config.DoLifetimeManagement = doLifetimeManagement;

            return config;
        }

        #endregion
    }
}
