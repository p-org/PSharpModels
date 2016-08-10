
using System.Threading.Tasks;

namespace Microsoft.PSharp.Actors.Bridge
{
    class WaitMachine<TResult> : Machine
    {
        #region events
        public class CompleteTask : Event
        {
            public ActorCompletionTask<TResult> InputTask;
            public MachineId Target;
            public int Timestamp;

            public CompleteTask(ActorCompletionTask<TResult> inputTask, MachineId target, int timestamp)
            {
                this.InputTask = inputTask;
                this.Target = target;
                this.Timestamp = timestamp;
            }
        }

        public class TaskCompleted : Event
        {
            public Task<TResult> ResultTask;
            public int Timestamp;

            public TaskCompleted(ActorCompletionTask<TResult> resultTask, int timestamp)
            {
                this.ResultTask = resultTask;
                this.Timestamp = timestamp;
            }
        }
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInit))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnInit()
        {
            var e = ReceivedEvent as CompleteTask;
            (e.InputTask).Wait();
            Send(e.Target, new TaskCompleted(e.InputTask, e.Timestamp));
        }
        #endregion
    }
}
