
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

            public CompleteTask(ActorCompletionTask<TResult> inputTask, MachineId target)
            {
                this.InputTask = inputTask;
                this.Target = target;
            }
        }

        public class TaskCompleted : Event
        {
            public Task<TResult> ResultTask;

            public TaskCompleted(ActorCompletionTask<TResult> resultTask)
            {
                this.ResultTask = resultTask;
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
            Send(e.Target, new TaskCompleted(e.InputTask));
        }
        #endregion
    }
}
