//-----------------------------------------------------------------------
// <copyright file="Actor.cs">
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

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public class Actor<TState> : ActorBase where TState : class
    {
        public TState State { get; set; }
    }

    /// <summary>
    /// Service fabric actor.
    /// </summary>
    public abstract class Actor : ActorBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected Actor()
        {

        }

        /// <summary>
        /// Gets the state manager that be used to
        /// get/add/update/remove named states.
        /// </summary>
        public IActorStateManager StateManager { get; set; }

        /// <summary>
        /// Saves all the state changes (add/update/remove) that were made since
        /// last call to Microsoft.ServiceFabric.Actors.Runtime.Actor.SaveStateAsync,
        /// to the actor state provider associated with the actor.
        /// </summary>
        /// <returns>Task that represents the asynchronous save operation.</returns>
        protected Task SaveStateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
