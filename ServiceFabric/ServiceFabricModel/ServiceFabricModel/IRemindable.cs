using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    //
    // Summary:
    //     Interface which actors that use reminders must implement.
    public interface IRemindable
    {
        //
        // Summary:
        //     Reminder call back invoked when an actor reminder is triggered.
        //
        // Parameters:
        //   reminderName:
        //     Name of reminder.
        //
        //   context:
        //     Context associated with this reminder was passed to Actors runtime when reminder
        //     was created.
        //
        //   dueTime:
        //     Time when reminder with name reminderName is due.
        //
        //   period:
        //     The time interval between triggering of reminder with name reminderName.
        //
        // Returns:
        //     A task that represents the asynchronous operation performed by this callback.
        //
        // Remarks:
        //     When a reminder is triggered, Actors runtime will invoke ReceiveReminderAsync
        //     method on the Actor. An actor can register multiple reminders and the ReceiveReminderAsync
        //     method is invoked any time any of those reminders is triggered. The actor can
        //     use the reminder name that is passed in to the ReceiveReminderAsync method to
        //     figure out which reminder was triggered.
        //     The Actors runtime saves the actor state when the ReceiveReminderAsync call completes.
        //     If an error occurs in saving the state, that actor object will be deactivated
        //     and a new instance will be activated. to specify that the state need not be saved
        //     upon completion of the reminder
        //     .
        Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period);
    }
}
