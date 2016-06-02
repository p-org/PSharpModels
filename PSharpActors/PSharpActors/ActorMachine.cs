//-----------------------------------------------------------------------
// <copyright file="ActorMachine.cs">
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
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.PSharp.Actors.Bridge;

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// A P# actor machine.
    /// </summary>
    public abstract class ActorMachine : Machine
    {
        #region events

        /// <summary>
        /// The actor initialization event.
        /// </summary>
        public class InitEvent : Event
        {
            public object ClassInstance;
            public Type ActorType;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="classInstance">ClassInstance</param>
            /// <param name="actorType">Type</param>
            public InitEvent(object classInstance, Type actorType)
            {
                this.ClassInstance = classInstance;
                this.ActorType = actorType;
            }
        }

        /// <summary>
        /// The actor invocation event.
        /// </summary>
        public class ActorEvent : Event
        {
            public Type MethodClass;
            public string MethodName;
            public object ClassInstance;
            public object[] Parameters;
            public MachineId ActorCompletionMachine;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="methodClass">MethodClass</param>
            /// <param name="methodName">MethodName</param>
            /// <param name="classInstance">ClassInstance</param>
            /// <param name="parameters">Parameters</param>
            /// <param name="tcs">TaskCompletionSource</param>
            public ActorEvent(Type methodClass, string methodName, object classInstance,
                object[] parameters, MachineId actorCompletionMachine)
            {
                this.MethodClass = methodClass;
                this.MethodName = methodName;
                this.ClassInstance = classInstance;
                this.Parameters = parameters;
                this.ActorCompletionMachine = actorCompletionMachine;
            }
        }

        #endregion

        #region fields

        /// <summary>
        /// The wrapped actor instance.
        /// </summary>
        protected object WrappedActorInstance;

        /// <summary>
        /// The wrapped actor type.
        /// </summary>
        protected Type WrappedActorType;

        /// <summary>
        /// Event that was buffered while the actor was inactive.
        /// It should be handled as soon as the actor becomes
        /// active.
        /// </summary>
        private Event BufferedEvent;

        /// <summary>
        /// Checks if the actor machine is active.
        /// </summary>
        internal bool IsActive;

        #endregion

        #region states

        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [DeferEvents(typeof(TimerMachine.TimeoutEvent), typeof(ReminderMachine.RemindEvent))]
        private class Init : MachineState { }

        [OnEntry(nameof(ActiveOnEntry))]
        [OnEventDoAction(typeof(ActorEvent), nameof(OnActorEvent))]
        [OnEventDoAction(typeof(TimerMachine.TimeoutEvent), nameof(HandleTimeout))]
        [OnEventDoAction(typeof(ReminderMachine.RemindEvent), nameof(HandleReminder))]
        [OnEventDoAction(typeof(Default), nameof(HandleDefaultAction))]
        private class Active : MachineState { }

        [OnEntry(nameof(InactiveOnEntry))]
        [OnEventDoAction(typeof(ActorEvent), nameof(BufferEventAndActivate))]
        [OnEventDoAction(typeof(ReminderMachine.RemindEvent), nameof(BufferEventAndActivate))]
        [IgnoreEvents(typeof(TimerMachine.TimeoutEvent))]
        private class Inactive : MachineState { }

        #endregion

        #region actions

        private void InitOnEntry()
        {
            var initEvent = this.ReceivedEvent as InitEvent;
            
            this.WrappedActorInstance = initEvent.ClassInstance;
            this.WrappedActorType = initEvent.ActorType;

            try
            {
                this.Initialize();
            }
            catch (TargetInvocationException ex)
            {
                throw ex;
            }
            
            this.Goto(typeof(Active));
        }

        private void ActiveOnEntry()
        {
            this.Activate();
            this.IsActive = true;
            
            if (this.BufferedEvent != null)
            {
                var eventToHandle = BufferedEvent;
                this.BufferedEvent = null;
                this.Raise(eventToHandle);
            }
        }

        private void InactiveOnEntry()
        {
            if (ActorModel.RegisteredTimers.ContainsKey(this.Id))
            {
                ActorModel.RegisteredTimers[this.Id].Clear();
            }

            this.Deactivate();
            this.IsActive = false;
        }

        private void OnActorEvent()
        {
            var actorEvent = (this.ReceivedEvent as ActorEvent);

            //For non-FIFO order.
            if (ActorModel.Configuration.DisableFirstInFirstOutOrder && Random())               
            {
                Send(this.Id, actorEvent);
            }
            else
            {
                // If FIFO order is disabled and multiple sends are enabled,
                // send nondeterministically a duplicate event to itself.
                if (ActorModel.Configuration.DisableFirstInFirstOutOrder &&
                    ActorModel.Configuration.DoMultipleSends && Random())
                {
                    Send(this.Id, actorEvent);
                }
                // Otherwise, if only multiple sends are enabled, send
                // nondeterministically a duplicate event to the executor.
                else if (ActorModel.Configuration.DoMultipleSends && Random())
                {
                    this.ExecuteActorAction(actorEvent);
                }

                this.ExecuteActorAction(actorEvent);
            }
        }

        private void ExecuteActorAction(ActorEvent actorEvent)
        {
            ActorModel.Runtime.Log($"<ActorModelLog> Machine '{base.Id.Name}' is invoking '{actorEvent.MethodName}'.");
            MethodInfo mi = actorEvent.MethodClass.GetMethod(actorEvent.MethodName);
            object result = mi.Invoke(actorEvent.ClassInstance, actorEvent.Parameters);
            this.Send(actorEvent.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(result));
        }

        private void HandleTimeout()
        {
            var timer = (this.ReceivedEvent as TimerMachine.TimeoutEvent).Timer;
            var callback = (this.ReceivedEvent as TimerMachine.TimeoutEvent).Callback;
            var callbackState = (this.ReceivedEvent as TimerMachine.TimeoutEvent).CallbackState;

            if (ActorModel.RegisteredTimers.ContainsKey(this.Id) &&
                ActorModel.RegisteredTimers[this.Id].Contains(timer))
            {
                ActorModel.Runtime.Log($"<ActorModelLog> Machine '{this.Id.Name}' is " +
                    $"handling timeout from timer '{timer.Name}'.");
                callback(callbackState);
            }
        }

        private void HandleReminder()
        {
            var cancellationSource = (this.ReceivedEvent as ReminderMachine.RemindEvent).CancellationSource;
            var reminderName = (this.ReceivedEvent as ReminderMachine.RemindEvent).ReminderName;
            var callbackState = (this.ReceivedEvent as ReminderMachine.RemindEvent).CallbackState;
            
            if (ActorModel.RegisteredReminders.ContainsKey(this.Id) &&
                ActorModel.RegisteredReminders[this.Id].Contains(cancellationSource))
            {
                ActorModel.Runtime.Log($"<ActorModelLog> Machine '{this.Id.Name}' is " +
                    $"handling timeout from reminder '{cancellationSource.Reminder.Name}'.");
                this.InvokeReminder(reminderName, callbackState);
            }
        }

        private void HandleDefaultAction()
        {
            // Deactivate the actor nondeterministically.
            if (ActorModel.Configuration.DoLifetimeManagement && this.Random())
            {
                this.Goto(typeof(Inactive));
            }
        }

        private void BufferEventAndActivate()
        {
            this.BufferedEvent = this.ReceivedEvent;
            this.Goto(typeof(Active));
        }

        protected abstract void Initialize();

        protected abstract void Activate();

        protected abstract void Deactivate();

        protected abstract void InvokeReminder(string reminderName, object callbackState);

        #endregion
    }
}
