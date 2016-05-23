//-----------------------------------------------------------------------
// <copyright file="ClientConfiguration.cs">
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

namespace Orleans.Runtime.Configuration
{
    /// <summary>
    /// Orleans client configuration parameters.
    /// </summary>
    public class ClientConfiguration
    {
        #region fields

        /// <summary>
        /// The client name.
        /// </summary>
        public static string ClientName;

        #endregion

        #region constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ClientConfiguration()
        {
            ClientConfiguration.ClientName = "Client";
        }

        #endregion

        #region methods

        /// <summary>
        /// Returns a ClientConfiguration object for
        /// connecting to a local silo.
        /// </summary>
        /// <param name="gatewayPort">GatewayPort</param>
        /// <returns>ClientConfiguration</returns>
        public static ClientConfiguration LocalhostSilo(int gatewayPort = 40000)
        {
            return new ClientConfiguration();
        }

        #endregion
    }
}
