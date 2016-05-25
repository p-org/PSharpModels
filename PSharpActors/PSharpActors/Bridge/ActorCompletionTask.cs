//-----------------------------------------------------------------------
// <copyright file="ActorCompletionTask.cs">
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
using System.Threading.Tasks;

namespace Microsoft.PSharp.Actors.Bridge
{
    public class ActorCompletionTask<TResult> : Task<TResult>
    {
        /// <summary>
        /// The P# runtime.
        /// </summary>
        private PSharpRuntime Runtime;

        /// <summary>
        /// Actor completion machine id.
        /// </summary>
        public MachineId ActorCompletionMachine { get; private set; }

        public new TResult Result
        {
            get
            {
                MachineId mid = this.Runtime.GetCurrentMachine();
                this.Runtime.SendEvent(this.ActorCompletionMachine, new ActorCompletionMachine.GetResultRequest(mid));
                Event resultEvent = this.Runtime.Receive(mid, typeof(ActorCompletionMachine.GetResultResponse));
                return (TResult)((Task<TResult>)(resultEvent as ActorCompletionMachine.GetResultResponse).Result).Result;
            }
        }

        public ActorCompletionTask(PSharpRuntime runtime)
            : base(() => { return default(TResult); })
        {
            this.Runtime = runtime;
            this.ActorCompletionMachine = this.Runtime.CreateMachine(typeof(ActorCompletionMachine));
        }

        public new void Wait()
        {
            MachineId mid = this.Runtime.GetCurrentMachine();
            this.Runtime.SendEvent(this.ActorCompletionMachine, new ActorCompletionMachine.GetResultRequest(mid));
            this.Runtime.Receive(mid, typeof(ActorCompletionMachine.GetResultResponse));
        }
    }
}
