//-----------------------------------------------------------------------
// <copyright file="TimerCancellationSource.cs">
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
    /// A time cancellation source.
    /// </summary>
    public class TimerCancellationSource : IDisposable
    {
        private MachineId Actor;
        public MachineId Timer { get; private set; }

        public TimerCancellationSource(MachineId actor, MachineId timer)
        {
            this.Actor = actor;
            this.Timer = timer;
        }

        public void Dispose()
        {
            ActorModel.Assert(ActorModel.Runtime.GetCurrentMachine().Equals(this.Actor),
                $"The timer can only be disposed by its owner, which is {this.Actor}." +
                $"Instead, {ActorModel.Runtime.GetCurrentMachine()} called Dispose().");

            if (ActorModel.ActorMachineMap[this.Actor].RegisteredTimers.Contains(this.Timer))
            {
                ActorModel.Runtime.Log($"<ActorModelLog> Machine '{this.Actor.Name}' is " +
                    $"unregistering timer '{this.Timer.Name}'.");
                ActorModel.ActorMachineMap[this.Actor].RegisteredTimers.Remove(this.Timer);
                ActorModel.Runtime.SendEvent(this.Timer, new Halt());
            }
        }
    }
}
