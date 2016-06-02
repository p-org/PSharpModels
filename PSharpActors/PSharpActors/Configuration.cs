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

        internal bool AllowReentrantCalls;
        
        internal bool AllowOutOfOrderSends;

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
            this.AllowReentrantCalls = true;
            this.PerformSerialization = true;
            this.DoMultipleSends = true;
            this.AllowOutOfOrderSends = true;
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
        /// <param name="allowReentrantCalls">Allow reentrant calls</param>
        /// <param name="performSerialization">Perform serialization</param>
        /// <param name="doMultipleSends">Do multiple sends</param>
        /// <param name="allowOutOfOrderSends">Allow out of order sends</param>
        /// <param name="doLifetimeManagement">Do lifetime management</param>
        /// <returns>Configuration</returns>
        public static Configuration Create(bool allowReentrantCalls, bool performSerialization, bool doMultipleSends,
            bool allowOutOfOrderSends, bool doLifetimeManagement)
        {
            var config = Configuration.Default();

            config.AllowReentrantCalls = allowReentrantCalls;
            config.PerformSerialization = performSerialization;
            config.DoMultipleSends = doMultipleSends;
            config.AllowOutOfOrderSends = allowOutOfOrderSends;
            config.DoLifetimeManagement = doLifetimeManagement;

            return config;
        }

        #endregion
    }
}
