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
using System.Reflection;

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
        /// Machine for executing actor operations.
        /// </summary>
        private MachineId executorMachine;

        /// <summary>
        /// Event that was buffered while the actor was inactive.
        /// It should be handled as soon as the actor becomes
        /// active.
        /// </summary>
        private ActorEvent EventToHandleOnActive;

        /// <summary>
        /// Checks if the actor machine is active.
        /// </summary>
        internal bool IsActive;

        #endregion

        #region states

        [Start]
        [OnEntry(nameof(InitOnEntry))]
        private class Init : MachineState { }

        [OnEntry(nameof(ActiveOnEntry))]
        [OnEventDoAction(typeof(ActorEvent), nameof(OnActorEvent))]
        [OnEventDoAction(typeof(Default), nameof(HandleDefaultAction))]
        private class Active : MachineState { }

        [OnEntry(nameof(InactiveOnEntry))]
        [OnEventDoAction(typeof(ActorEvent), nameof(BufferEventAndActivate))]
        private class Inactive : MachineState { }

        #endregion

        #region actions

        private void InitOnEntry()
        {
            var initEvent = this.ReceivedEvent as InitEvent;

            this.WrappedActorInstance = initEvent.ClassInstance;
            this.WrappedActorType = initEvent.ActorType;

            executorMachine = CreateMachine(typeof(ActorExecutorMachine),
                initEvent.ActorType.FullName);

            try
            {
                this.InitializeState();
            }
            catch (TargetInvocationException ex)
            {
                throw new ActorModelException(ex.ToString());
            }
            
            this.Goto(typeof(Active));
        }

        private void ActiveOnEntry()
        {
            try
            {
                this.Activate();
            }
            catch (TargetInvocationException ex)
            {
                throw new ActorModelException(ex.ToString());
            }

            this.IsActive = true;
            
            if (this.EventToHandleOnActive != null)
            {
                var eventToHandle = this.EventToHandleOnActive;
                this.EventToHandleOnActive = null;
                this.Raise(eventToHandle);
            }
        }

        private void InactiveOnEntry()
        {
            try
            {
                this.Deactivate();
            }
            catch (TargetInvocationException ex)
            {
                throw new ActorModelException(ex.ToString());
            }

            this.IsActive = false;
        }

        private void OnActorEvent()
        {
            var e = (this.ReceivedEvent as ActorEvent);

            //For non-FIFO order.
            if (ActorModel.Configuration.DisableFirstInFirstOutOrder && Random())               
            {
                Send(this.Id, e);
            }
            else
            {
                // If FIFO order is disabled and multiple sends are enabled,
                // send nondeterministically a duplicate event to itself.
                if (ActorModel.Configuration.DisableFirstInFirstOutOrder &&
                    ActorModel.Configuration.DoMultipleSends && Random())
                {
                    Send(this.Id, e);
                }
                // Otherwise, if only multiple sends are enabled, send
                // nondeterministically a duplicate event to the executor.
                else if (ActorModel.Configuration.DoMultipleSends && Random())
                {
                    Send(executorMachine, e);
                }

                Send(executorMachine, e);
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
            this.EventToHandleOnActive = this.ReceivedEvent as ActorEvent;
            this.Goto(typeof(Active));
        }

        protected abstract void InitializeState();

        protected abstract void Activate();

        protected abstract void Deactivate();

        #endregion
    }
}
