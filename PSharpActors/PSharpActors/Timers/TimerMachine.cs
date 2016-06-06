//-----------------------------------------------------------------------
// <copyright file="TimerMachine.cs">
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
using System.Threading.Tasks;

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// A P# actor timer machine.
    /// </summary>
    public class TimerMachine : Machine
    {
        #region events

        /// <summary>
        /// The timer initialization event.
        /// </summary>
        public class InitEvent : Event
        {
            public MachineId Target;
            public Func<object, Task> Callback;
            public object CallbackState;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="target">MachineId</param>
            /// <param name="callback">Callback</param>
            /// <param name="callbackState">State</param>
            public InitEvent(MachineId target, Func<object, Task> callback, object callbackState)
            {
                this.Target = target;
                this.Callback = callback;
                this.CallbackState = callbackState;
            }
        }

        /// <summary>
        /// The timeout event.
        /// </summary>
        public class TimeoutEvent : Event
        {
            public MachineId Timer;
            public Func<object, Task> Callback;
            public object CallbackState;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="timer">MachineId</param>
            /// <param name="callback">Callback</param>
            /// <param name="callbackState">State</param>
            public TimeoutEvent(MachineId timer, Func<object, Task> callback, object callbackState)
            {
                this.Timer = timer;
                this.Callback = callback;
                this.CallbackState = callbackState;
            }
        }

        #endregion

        #region fields

        private MachineId Target;

        private Func<object, Task> Callback;

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
            this.Callback = (this.ReceivedEvent as InitEvent).Callback;
            this.CallbackState = (this.ReceivedEvent as InitEvent).CallbackState;
            
            ActorModel.ActorMachineMap[this.Target].RegisteredTimers.Add(this.Id);

            this.Goto(typeof(Active));
        }

        private void HandleDefaultAction()
        {
            if (this.Random())
            {
                this.Send(this.Target, new TimeoutEvent(this.Id, this.Callback, this.CallbackState));
            }
        }

        #endregion
    }
}
