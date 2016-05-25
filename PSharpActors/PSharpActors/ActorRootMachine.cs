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
        public class RunEvent : Event
        {
            public Action Action;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="action">Action</param>
            public RunEvent(Action action)
            {
                this.Action = action;
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
            (this.ReceivedEvent as RunEvent).Action();
        }

        #endregion
    }
}
