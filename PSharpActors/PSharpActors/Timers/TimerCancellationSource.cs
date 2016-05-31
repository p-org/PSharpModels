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
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors.Bridge;

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// A time cancellation source.
    /// </summary>
    public class TimerCancellationSource : IDisposable
    {
        private MachineId Actor;
        private MachineId Timer;

        public TimerCancellationSource(MachineId actor, MachineId timer)
        {
            this.Actor = actor;
            this.Timer = timer;
        }

        void IDisposable.Dispose()
        {
            ActorModel.Assert(ActorModel.Runtime.GetCurrentMachine().Equals(this.Actor),
                $"The timer can only be disposed by its owner, which is {this.Actor}." +
                $"Instead, {ActorModel.Runtime.GetCurrentMachine()} called Dispose().");

            if (ActorModel.RegisteredTimers.ContainsKey(this.Actor) &&
                ActorModel.RegisteredTimers[this.Actor].Contains(this.Timer))
            {
                ActorModel.Runtime.Log($"<ActorModelLog> Machine '{this.Actor.Name}' is " +
                    $"unregistering timer '{this.Timer.Name}'.");
                ActorModel.RegisteredTimers[this.Actor].Remove(this.Timer);
                ActorModel.Runtime.SendEvent(this.Timer, new Halt());
            }
        }
    }
}
