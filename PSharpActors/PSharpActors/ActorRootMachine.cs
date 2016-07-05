//-----------------------------------------------------------------------
// <copyright file="ActorRootMachine.cs">
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

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// The P# actor root machine.
    /// </summary>
    internal class ActorRootMachine : Machine
    {
        #region events

        /// <summary>
        /// The actor root machine run event.
        /// </summary>
        public class Configure : Event
        {
            public Action EntryPoint;
            public ISet<Action> CleanUpActions;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="entryPoint">Action</param>
            /// <param name="cleanUpActions">Cleanup actions</param>
            public Configure(Action entryPoint, ISet<Action> cleanUpActions)
            {
                this.EntryPoint = entryPoint;
                this.CleanUpActions = cleanUpActions;
            }
        }
        
        #endregion

        #region states

        [Start]
        [OnEntry(nameof(InitEntry))]
        private class Init : MachineState { }

        #endregion

        #region actions

        private void InitEntry()
        {
            // Does the cleanup before a new testing iteration starts.
            var cleanupActions = (this.ReceivedEvent as Configure).CleanUpActions;
            foreach (var cleanupAction in cleanupActions)
            {
                cleanupAction();
            }

            // Invokes the entry point of the actor program.
            (this.ReceivedEvent as Configure).EntryPoint();
        }

        #endregion
    }
}
