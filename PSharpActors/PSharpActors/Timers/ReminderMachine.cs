//-----------------------------------------------------------------------
// <copyright file="ReminderMachine.cs">
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

using System.Collections.Generic;

using Microsoft.PSharp.Actors.Bridge;

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// An abstract P# actor reminder machine.
    /// </summary>
    public abstract class ReminderMachine : Machine
    {
        #region events

        /// <summary>
        /// The reminder initialization event.
        /// </summary>
        public class InitEvent : Event
        {
            public MachineId Target;
            public MachineId ActorCompletionMachine;
            public string ReminderName;
            public object CallbackState;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="target">MachineId</param>
            /// <param name="actorCompletionMachine">MachineId</param>
            /// <param name="reminderName">string</param>
            /// <param name="callbackState">State</param>
            public InitEvent(MachineId target, MachineId actorCompletionMachine,
                string reminderName, object callbackState)
            {
                this.Target = target;
                this.ActorCompletionMachine = actorCompletionMachine;
                this.ReminderName = reminderName;
                this.CallbackState = callbackState;
            }
        }

        /// <summary>
        /// The remind event.
        /// </summary>
        public class RemindEvent : Event
        {
            public ReminderCancellationSource CancellationSource;
            public string ReminderName;
            public object CallbackState;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="cancellationSource">ReminderCancellationSource</param>
            /// <param name="reminderName">string</param>
            /// <param name="callbackState">State</param>
            public RemindEvent(ReminderCancellationSource cancellationSource,
                string reminderName, object callbackState)
            {
                this.CancellationSource = cancellationSource;
                this.ReminderName = reminderName;
                this.CallbackState = callbackState;
            }
        }

        #endregion

        #region fields

        protected MachineId Target;

        private ReminderCancellationSource CancellationSource;

        protected string ReminderName;

        private object CallbackState;

        #endregion

        #region states

        [Start]
        [OnEntry(nameof(InitOnEntry))]
        private class Init : MachineState { }

        [OnEventDoAction(typeof(Default), nameof(HandleDefaultAction))]
        private class Active : MachineState { }

        #endregion

        #region actions

        private void InitOnEntry()
        {
            this.Target = (this.ReceivedEvent as InitEvent).Target;
            this.ReminderName = (this.ReceivedEvent as InitEvent).ReminderName;
            this.CallbackState = (this.ReceivedEvent as InitEvent).CallbackState;

            if (!ActorModel.RegisteredReminders.ContainsKey(this.Target))
            {
                ActorModel.RegisteredReminders.Add(this.Target,
                    new HashSet<ReminderCancellationSource>());
            }

            var cancellationSource = this.CreateReminderCancellationSource();
            ActorModel.RegisteredReminders[this.Target].Add(
                cancellationSource as ReminderCancellationSource);

            this.Send((this.ReceivedEvent as InitEvent).ActorCompletionMachine,
                new ActorCompletionMachine.SetResultRequest(cancellationSource));

            this.Goto(typeof(Active));
        }

        private void HandleDefaultAction()
        {
            if (this.Random())
            {
                this.Send(this.Target, new RemindEvent(this.CancellationSource,
                    this.ReminderName, this.CallbackState));
            }
        }

        protected abstract object CreateReminderCancellationSource();

        #endregion
    }
}
