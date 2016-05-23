//-----------------------------------------------------------------------
// <copyright file="InMemoryStorage.cs">
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

using Orleans.Core;

namespace OrleansModel
{
    /// <summary>
    /// A simple in-memory storage.
    /// </summary>
    internal class InMemoryStorage : IStorage
    {
        #region methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public InMemoryStorage()
        {

        }

        #endregion

        #region methods

        /// <summary>
        /// Async method to cause the current grain state data
        /// to be cleared and reset. This will usually mean the
        /// state record is deleted from backing store, but the
        /// specific behavior is defined by the storage provider
        /// instance configured for this grain.
        /// </summary>
        /// <returns>Task</returns>
        Task IStorage.ClearStateAsync()
        {
            return DummyTask.FromResult(true);
        }

        /// <summary>
        /// Async method to cause write of the current
        /// grain state data into backing store.
        /// </summary>
        /// <returns>Task</returns>
        Task IStorage.WriteStateAsync()
        {
            return DummyTask.FromResult(true);
        }

        /// <summary>
        /// Async method to cause refresh of the current
        /// grain state data from backing store.
        /// </summary>
        /// <returns>Task</returns>
        Task IStorage.ReadStateAsync()
        {
            return DummyTask.FromResult(true);
        }

        #endregion
    }
}