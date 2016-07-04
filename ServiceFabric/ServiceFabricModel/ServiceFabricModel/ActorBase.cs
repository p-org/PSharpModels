//-----------------------------------------------------------------------
// <copyright file="ActorBase.cs">
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

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.PSharp.Actors.Bridge;
using ServiceFabricModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    public abstract class ActorBase
    {
        //
        // Summary:
        //     Gets the stateful service replica that is hosting the actor.
        //public ActorService ActorService { get; }

        /// <summary>
        /// Gets the name of the application that contains the
        /// actor service that is hosting this actor.
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets the identity of this actor with the actor service.
        /// </summary>
        public ActorId Id { get; internal set; }

        /// <summary>
        /// Gets the Uri of the actor service that is hosting this actor.
        /// </summary>
        public Uri ServiceUri { get; }

        //
        // Summary:
        //     Gets the event for the specified event interface.
        //
        // Type parameters:
        //   TEvent:
        //     Event interface type.
        //
        // Returns:
        //     Returns Event that represents the specified interface.
        //protected TEvent GetEvent<TEvent>();
        //
        // Summary:
        //     Gets the actor reminder with specified reminder name.
        //
        // Parameters:
        //   reminderName:
        //     Name of the reminder to get.
        //
        // Returns:
        //     An Microsoft.ServiceFabric.Actors.Runtime.IActorReminder that represents an actor
        //     reminder.
        //protected IActorReminder GetReminder(string reminderName);
        
        /// <summary>
        /// Override this method to initialize the members, initialize
        /// state or register timers. This method is called right after
        /// the actor is activated and before any method call or reminders
        /// are dispatched on it.
        /// </summary>
        /// <returns>Task</returns>
        protected virtual Task OnActivateAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Override this method to release any resources including
        /// unregistering the timers. This method is called right
        /// before the actor is deactivated.
        /// </summary>
        /// <returns>Task</returns>
        protected virtual Task OnDeactivateAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Registers the specified reminder.
        /// </summary>
        /// <param name="reminderName">Name of the reminder to register</param>
        /// <param name="state">State associated with reminder</param>
        /// <param name="dueTime">TimeSpan when actor timer is first due</param>
        /// <param name="period">TimeSpan for subsequent actor timer invocation</param>
        /// <returns>IActorReminder</returns>
        protected Task<IActorReminder> RegisterReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            var reminders = ActorModel.GetReminders(ActorModel.Runtime.GetCurrentMachine());
            var reminder = reminders.SingleOrDefault(val => ((ActorReminder)val).Name.Equals(reminderName));

            var task = new ActorCompletionTask<IActorReminder>();
            var actorCompletionMachine = task.ActorCompletionMachine;
            if (reminder != null)
            {
                ActorModel.Runtime.SendEvent(actorCompletionMachine,
                    new ActorCompletionMachine.SetResultRequest(reminder));
            }
            else
            {
                ActorModel.Runtime.CreateMachine(typeof(ActorReminderMachine), reminderName,
                    new ReminderMachine.InitEvent(ActorModel.Runtime.GetCurrentMachine(),
                    actorCompletionMachine, reminderName, null));
            }

            return task;
        }

        /// <summary>
        /// Registers a Timer for the actor.
        /// </summary>
        /// <param name="asyncCallback">Callback to invoke when timer fires</param>
        /// <param name="state">State to pass into timer callback</param>
        /// <param name="dueTime">TimeSpan when actor timer is first due</param>
        /// <param name="period">TimeSpan for subsequent actor timer invocation</param>
        /// <returns>IActorTimer</returns>
        protected IActorTimer RegisterTimer(Func<object, Task> asyncCallback,
            object state, TimeSpan dueTime, TimeSpan period)
        {
            MachineId timer = ActorModel.Runtime.CreateMachine(typeof(TimerMachine),
                new TimerMachine.InitEvent(ActorModel.Runtime.GetCurrentMachine(),
                asyncCallback, state));
            return new ActorTimer(dueTime, period, ActorModel.Runtime.GetCurrentMachine(), timer);
        }

        /// <summary>
        /// Gets the actor reminder with specified reminder name.
        /// </summary>
        /// <param name="reminderName">Name</param>
        /// <returns>IActorReminder</returns>
        protected IActorReminder GetReminder(string reminderName)
        {
            var reminders = ActorModel.GetReminders(ActorModel.Runtime.GetCurrentMachine());
            var reminder = reminders.SingleOrDefault(val => ((ActorReminder)val).Name.Equals(reminderName));

            return (IActorReminder)reminder;
        }

        /// <summary>
        /// Unregisters the specified reminder.
        /// </summary>
        /// <param name="reminder">IActorReminder</param>
        /// <returns>Task</returns>
        protected Task UnregisterReminderAsync(IActorReminder reminder)
        {
            ActorModel.Assert(reminder != null, "Cannot unregister a 'null' reminder.");
            var reminders = ActorModel.GetReminders(ActorModel.Runtime.GetCurrentMachine());
            var reminderToBeRemoved = reminders.SingleOrDefault(val
                => ((ActorReminder)val).Name.Equals(reminder.Name));
            if (reminderToBeRemoved != null)
            {
                reminderToBeRemoved.Dispose();
            }

            var task = new ActorCompletionTask<object>();
            var actorCompletionMachine = task.ActorCompletionMachine;
            ActorModel.Runtime.SendEvent(actorCompletionMachine,
                new ActorCompletionMachine.SetResultRequest(true));
            return task;
        }

        /// <summary>
        /// Unregisters a Timer previously set on this actor.
        /// </summary>
        /// <param name="timer">IActorTimer</param>
        protected void UnregisterTimer(IActorTimer timer)
        {
            ((ActorTimer)timer).Dispose();
        }
    }
}
