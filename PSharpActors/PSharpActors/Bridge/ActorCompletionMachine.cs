//-----------------------------------------------------------------------
// <copyright file="ActorCompletionMachine.cs">
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

namespace Microsoft.PSharp.Actors.Bridge
{
    /// <summary>
    /// The actor completion machine.
    /// </summary>
    public class ActorCompletionMachine : Machine
    {
        /// <summary>
        /// Set result request event.
        /// </summary>
        public class SetResultRequest : Event
        {
            public object Result;

            public SetResultRequest(object result)
            {
                this.Result = result;
            }
        }

        /// <summary>
        /// Get result request event.
        /// </summary>
        public class GetResultRequest : Event
        {
            public MachineId Target;

            public GetResultRequest(MachineId target)
            {
                this.Target = target;
            }
        }

        /// <summary>
        /// Get result response event.
        /// </summary>
        public class GetResultResponse : Event
        {
            public MachineId Source;
            public object Result;

            public GetResultResponse(MachineId source, object result)
            {
                this.Source = source;
                this.Result = result;
            }
        }

        /// <summary>
        /// The target machine.
        /// </summary>
        private MachineId Target;

        /// <summary>
        /// The result.
        /// </summary>
        private object Result;
        
        [Start]
        [OnEntry(nameof(InitOnAction))]
        private class Init : MachineState { }

        private void InitOnAction()
        {
            this.Result = null;
            this.Goto(typeof(Active));
        }

        [OnEventDoAction(typeof(SetResultRequest), nameof(HandleSetResultRequest))]
        [OnEventDoAction(typeof(GetResultRequest), nameof(HandleGetResultRequest))]
        private class Active : MachineState { }

        /// <summary>
        /// Handles the set result request event.
        /// </summary>
        private void HandleSetResultRequest()
        {
            this.Result = (this.ReceivedEvent as SetResultRequest).Result;
            if (this.Target != null)
            {
                this.Send(this.Target, new GetResultResponse(this.Id, this.Result));
                //this.Raise(new Halt());
            }
        }

        /// <summary>
        /// Handles the get result request event.
        /// </summary>
        private void HandleGetResultRequest()
        {
            this.Target = (this.ReceivedEvent as GetResultRequest).Target;
            if (this.Result != null)
            {
                this.Send(this.Target, new GetResultResponse(this.Id, this.Result));
                //this.Raise(new Halt());
            }
        }
    }
}
