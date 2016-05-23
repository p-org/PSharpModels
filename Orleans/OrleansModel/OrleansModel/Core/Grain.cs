//-----------------------------------------------------------------------
// <copyright file="Grain.cs">
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

using System.Threading.Tasks;

using Microsoft.PSharp.Actors.Bridge;

using Orleans.Runtime;

namespace Orleans
{
    /// <summary>
    /// The abstract base class for all grain classes.
    /// </summary>
    public abstract class Grain : IAddressable
    {
        #region fields

        /// <summary>
        /// The grain factory.
        /// </summary>
        protected IGrainFactory GrainFactory { get; }

        #endregion

        #region constructors

        /// <summary>
        /// Constructor. This constructor should never be invoked. We
        /// expose it so that client code (subclasses of Grain) do not
        /// have to add a constructor. Client code should use the
        /// GrainFactory property to get a reference to a Grain.
        /// </summary>
        public Grain()
        {
            this.GrainFactory = GrainClient.GrainFactory;
        }

        #endregion

        #region methods

        /// <summary>
        /// This method is called at the end of the process of
        /// activating a grain. It is called before any messages
        /// have been dispatched to the grain. For grains with
        /// declared persistent state, this method is called
        /// after the State property has been populated.
        /// </summary>
        /// <returns>Task</returns>
        public virtual Task OnActivateAsync()
        {
            return DummyTask.FromResult(true);
        }

        /// <summary>
        /// This method is called at the begining of the
        /// process of deactivating a grain.
        /// </summary>
        /// <returns>Task</returns>
        public virtual Task OnDeactivateAsync()
        {
            return DummyTask.FromResult(true);
        }

        #endregion
    }
}
