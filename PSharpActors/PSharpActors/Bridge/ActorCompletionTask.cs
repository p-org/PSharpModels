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

using System.Threading.Tasks;

namespace Microsoft.PSharp.Actors.Bridge
{
    public class ActorCompletionTask<TResult> : Task<TResult>
    {
        /// <summary>
        /// Actor completion machine id.
        /// </summary>
        public MachineId ActorCompletionMachine { get; private set; }

        /// <summary>
        /// The result of the task.
        /// </summary>
        public new TResult Result
        {
            get
            {
                MachineId mid = ActorModel.Runtime.GetCurrentMachine();
                ActorModel.Runtime.SendEvent(this.ActorCompletionMachine,
                    new ActorCompletionMachine.GetResultRequest(mid));
                Event resultEvent = ActorModel.Runtime.Receive(mid,
                    typeof(ActorCompletionMachine.GetResultResponse));
                return (TResult)((Task<TResult>)(resultEvent as
                    ActorCompletionMachine.GetResultResponse).Result).Result;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActorCompletionTask()
            : base(() => { return default(TResult); })
        {
            this.ActorCompletionMachine = ActorModel.Runtime.CreateMachine(
                typeof(ActorCompletionMachine));
        }

        /// <summary>
        /// Wait the task to complete.
        /// </summary>
        public new void Wait()
        {
            MachineId mid = ActorModel.Runtime.GetCurrentMachine();
            ActorModel.Runtime.SendEvent(this.ActorCompletionMachine,
                new ActorCompletionMachine.GetResultRequest(mid));
            ActorModel.Runtime.Receive(mid, typeof(ActorCompletionMachine.GetResultResponse));
        }
    }
}
