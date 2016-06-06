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
using System.Linq;
using System.Reflection;

using Microsoft.PSharp.Actors.Bridge;
using Microsoft.PSharp.Actors.Utilities;

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

            public MachineId Sender;
            public List<MachineId> ExecutionContext;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="methodClass">MethodClass</param>
            /// <param name="methodName">MethodName</param>
            /// <param name="classInstance">ClassInstance</param>
            /// <param name="parameters">Parameters</param>
            /// <param name="tcs">TaskCompletionSource</param>
            /// <param name="senderMachineId">Machine id of sender</param>
            public ActorEvent(Type methodClass, string methodName, object classInstance,
                object[] parameters, MachineId actorCompletionMachine, MachineId senderMachineId = null)
            {
                this.MethodClass = methodClass;
                this.MethodName = methodName;
                this.ClassInstance = classInstance;
                this.Parameters = parameters;
                this.ActorCompletionMachine = actorCompletionMachine;
                this.ExecutionContext = new List<MachineId>();
                
                if (senderMachineId == null)
                {
                    this.Sender = ActorModel.Runtime.GetCurrentMachine();
                }
                else
                {
                    this.Sender = senderMachineId;
                }

                if (ActorModel.ActorMachineMap.ContainsKey(this.Sender))
                {
                    this.ExecutionContext = ActorModel.ActorMachineMap[this.Sender].LatestExecutionContext.ToList();
                }
                
                this.ExecutionContext.Add(this.Sender);
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

        /// <summary>
        /// Set of registered timers.
        /// </summary>
        internal ISet<MachineId> RegisteredTimers;

        /// <summary>
        ///Set of registered reminders.
        /// </summary>
        internal ISet<ReminderCancellationSource> RegisteredReminders;

        /// <summary>
        /// Latest calling context.
        /// </summary>
        internal List<MachineId> LatestExecutionContext;

        /// <summary>
        /// Reentrant action handler.
        /// </summary>
        internal Action<ActorMachine.ActorEvent> ReentrantActionHandler;

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

            this.RegisteredTimers = new HashSet<MachineId>();
            this.RegisteredReminders = new HashSet<ReminderCancellationSource>();
            this.LatestExecutionContext = new List<MachineId>();

            ActorModel.ActorMachineMap.Add(this.Id, this);

            this.WrappedActorInstance = initEvent.ClassInstance;
            this.WrappedActorType = initEvent.ActorType;
            
            try
            {
                this.Initialize();
                if (this.IsReentrant())
                {
                    ActorModel.ReentrantActors.Add(this.Id, true);
                    this.ReentrantActionHandler = HandleActorEvent;
                }
                else
                {
                    ActorModel.ReentrantActors.Add(this.Id, false);
                }
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
            this.RegisteredTimers.Clear();
            this.Deactivate();
            this.IsActive = false;
        }

        private void OnActorEvent()
        {
            var actorEvent = (this.ReceivedEvent as ActorEvent);
            this.HandleActorEvent(actorEvent);
        }

        private void HandleActorEvent(ActorEvent actorEvent)
        {
            //For non-FIFO order.
            if (ActorModel.Configuration.AllowOutOfOrderSends && Random())
            {
                Send(this.Id, actorEvent);

            }
            else
            {
                // If multiple sends are enabled, send nondeterministically
                // a duplicate event to itself.
                if (ActorModel.Configuration.DoMultipleSends && Random())
                {
                    var duplicateEvent = new ActorEvent(actorEvent.MethodClass, actorEvent.MethodName,
                        actorEvent.ClassInstance, Serialization.Serialize(actorEvent.Parameters),
                        actorEvent.ActorCompletionMachine, actorEvent.Sender);
                    Send(this.Id, duplicateEvent);
                }

                this.ExecuteActorAction(actorEvent);
            }
        }

        private void ExecuteActorAction(ActorEvent actorEvent)
        {
            this.LatestExecutionContext = actorEvent.ExecutionContext.ToList();
            
            foreach (var x in (actorEvent as ActorMachine.ActorEvent).ExecutionContext)
            {
                Console.WriteLine(" >> " + x.Name);
            }

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

            if (this.RegisteredTimers.Contains(timer))
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
            
            if (this.RegisteredReminders.Contains(cancellationSource))
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

        protected abstract bool IsReentrant();

        #endregion
    }
}
