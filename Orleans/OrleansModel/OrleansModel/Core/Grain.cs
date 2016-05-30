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

using System;
using System.Threading.Tasks;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors.Bridge;

using Orleans.Core;
using Orleans.Runtime;

using OrleansModel;

namespace Orleans
{
    /// <summary>
    /// The abstract base class for all grain classes.
    /// </summary>
    public abstract class Grain : IAddressable
    {
        #region fields

        /// <summary>
        /// The grain identity.
        /// </summary>
        internal IGrainIdentity Identity { get; private set; }

        /// <summary>
        /// The grain runtime.
        /// </summary>
        internal IGrainRuntime Runtime { get; private set; }

        /// <summary>
        /// The grain factory.
        /// </summary>
        protected IGrainFactory GrainFactory
        {
            get { return Runtime.GrainFactory; }
        }

        /// <summary>
        /// The service factory.
        /// </summary>
        protected IServiceProvider ServiceProvider
        {
            get { return Runtime.ServiceProvider; }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Constructor. This constructor should never be invoked. We
        /// expose it so that client code (subclasses of Grain) do not
        /// have to add a constructor. Client code should use the
        /// GrainFactory property to get a reference to a Grain.
        /// </summary>
        protected Grain()
        {

        }

        /// <summary>
        /// Constructor. Grain implementers do not have to expose this
        /// constructor but can choose to do so. This constructor is
        /// particularly useful for unit testing where test code can
        /// create a Grain and replace the IGrainIdentity and IGrainRuntime
        /// with test doubles.
        /// </summary>
        /// <param name="identity">IGrainIdentity</param>
        /// <param name="runtime">IGrainRuntime</param>
        protected Grain(IGrainIdentity identity, IGrainRuntime runtime)
        {
            this.Identity = identity;
            this.Runtime = runtime;
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
            return Task.FromResult(true);
        }

        /// <summary>
        /// This method is called at the begining of the
        /// process of deactivating a grain.
        /// </summary>
        /// <returns>Task</returns>
        public virtual Task OnDeactivateAsync()
        {
            return Task.FromResult(true);
        }

        #endregion
    }

    /// <summary>
    /// The abstract base class for all grain classes
    /// with declared persistent state.
    /// </summary>
    public class Grain<TGrainState> : Grain, IStatefulGrain
    {
        /// <summary>
        /// The grain state.
        /// </summary>
        private readonly GrainState<TGrainState> GrainState;

        IGrainState IStatefulGrain.GrainState
        {
            get
            {
                return this.GrainState;
            }
        }

        /// <summary>
        /// The grain storage.
        /// </summary>
        private IStorage Storage;

        /// <summary>
        /// The grain state.
        /// </summary>
        protected TGrainState State
        {
            get { return this.GrainState.State; }
            set { this.GrainState.State = value; }
        }

        /// <summary>
        /// Constructor. This constructor should never be invoked. We
        /// expose it so that client code (subclasses of Grain) do not
        /// have to add a constructor. Client code should use the
        /// GrainFactory property to get a reference to a Grain.
        /// </summary>
        protected Grain()
            : base()
        {
            this.GrainState = new GrainState<TGrainState>();
        }

        /// <summary>
        /// Constructor. Grain implementers do not have to expose this
        /// constructor but can choose to do so. This constructor is
        /// particularly useful for unit testing where test code can
        /// create a Grain and replace the IGrainIdentity and IGrainRuntime
        /// with test doubles.
        /// </summary>
        /// <param name="identity">IGrainIdentity</param>
        /// <param name="runtime">IGrainRuntime</param>
        /// <param name="state">TGrainState</param>
        /// <param name="storage">IStorage</param>
        protected Grain(IGrainIdentity identity, IGrainRuntime runtime,
            TGrainState state, IStorage storage)
            : base(identity, runtime)
        {
            this.GrainState = new GrainState<TGrainState>(state);
            this.Storage = storage;
        }

        /// <summary>
        /// Sets the storage of the grain to the
        /// specified storage.
        /// </summary>
        /// <param name="storage">IStorage</param>
        void IStatefulGrain.SetStorage(IStorage storage)
        {
            this.Storage = storage;
        }

        /// <summary>
        /// Async method to cause the current grain state data
        /// to be cleared and reset.  This will usually mean
        /// the state record is deleted from backing store, but
        /// the specific behavior is defined by the storage
        /// provider instance configured for this grain.
        /// </summary>
        /// <returns>Task</returns>
        protected virtual Task ClearStateAsync()
        {
            return this.Storage.ClearStateAsync();
        }

        /// <summary>
        /// Async method to cause write of the current
        /// grain state data into backing store.
        /// </summary>
        /// <returns></returns>
        protected virtual Task WriteStateAsync()
        {
            return this.Storage.WriteStateAsync();
        }

        /// <summary>
        /// Async method to cause refresh of the current grain
        /// state data from backing store. Any previous contents
        /// of the grain state data will be overwritten.
        /// </summary>
        /// <returns></returns>
        protected virtual Task ReadStateAsync()
        {
            return this.Storage.ReadStateAsync();
        }
    }
}
