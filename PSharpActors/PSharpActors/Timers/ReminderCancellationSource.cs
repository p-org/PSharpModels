//-----------------------------------------------------------------------
// <copyright file="ReminderCancellationSource.cs">
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
    /// A reminder cancellation source.
    /// </summary>
    public class ReminderCancellationSource : IDisposable
    {
        private MachineId Actor;
        public MachineId Reminder { get; private set; }

        public ReminderCancellationSource(MachineId actor, MachineId reminder)
        {
            this.Actor = actor;
            this.Reminder = reminder;
        }

        public void Dispose()
        {
            ActorModel.Assert(ActorModel.Runtime.GetCurrentMachineId().Equals(this.Actor),
                $"The reminder can only be disposed by its owner, which is {this.Actor}." +
                $"Instead, {ActorModel.Runtime.GetCurrentMachineId()} called Dispose().");

            if (ActorModel.ActorMachineMap[this.Actor].RegisteredReminders.Contains(this))
            {
                ActorModel.Runtime.Log($"<ActorModelLog> Machine '{this.Actor.Name}' is " +
                    $"unregistering reminder '{this.Reminder.Name}'.");
                ActorModel.ActorMachineMap[this.Actor].RegisteredReminders.Remove(this);
                ActorModel.Runtime.SendEvent(this.Reminder, new Halt());
            }
        }
    }
}
