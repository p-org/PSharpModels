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
                Event resultEvent = this.WaitOnEvent();

                var result = (resultEvent as ActorCompletionMachine.GetResultResponse).Result;
                if (result is Task<TResult>)
                {
                    return ((Task<TResult>)result).Result;
                }
                else
                {
                    return (TResult)result;
                }
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
            this.WaitOnEvent();
        }

        private Event WaitOnEvent()
        {
            MachineId mid = ActorModel.Runtime.GetCurrentMachine();

            ActorModel.Runtime.Log($"<ActorModelLog> Machine '{mid.Name}' is " +
                "waiting to receive a result.");

            ActorModel.Runtime.SendEvent(this.ActorCompletionMachine,
                new ActorCompletionMachine.GetResultRequest(mid));

            Event resultEvent = null;
            bool receivedResult = false;
            while (!receivedResult)
            {
                resultEvent = ActorModel.Runtime.Receive(
                    Tuple.Create<Type, Func<Event, bool>, Action<Event>>(typeof(ActorCompletionMachine.GetResultResponse), (Event e) =>
                    {
                        if ((e as ActorCompletionMachine.GetResultResponse).Source.Equals(this.ActorCompletionMachine))
                        {
                            return true;
                        }

                        return false;
                    },
                    (Event e) => { receivedResult = true; }),
                    Tuple.Create<Type, Func<Event, bool>, Action<Event>>(typeof(ActorMachine.ActorEvent), (Event e) =>
                    {
                        foreach (var x in (e as ActorMachine.ActorEvent).ExecutionContext)
                        {
                            Console.WriteLine(" >>>> " + x.Name + " " + (e as ActorMachine.ActorEvent).MethodName);
                        }

                        if (ActorModel.Configuration.AllowReentrantCalls &&
                            ActorModel.ReentrantActors.ContainsKey(mid) &&
                            ActorModel.ReentrantActors[mid])
                        {
                            return true;
                        }

                        ActorModel.Assert(!(e as ActorMachine.ActorEvent).ExecutionContext.Contains(mid),
                            $"Deadlock detected. {mid.Name} is not reentrant.");

                        return false;
                    },
                    (Event e) => {
                            var handler = ActorModel.GetReentrantActionHandler(mid);
                            handler(e as ActorMachine.ActorEvent);
                    }));
            }

            return resultEvent;
        }
    }
}
