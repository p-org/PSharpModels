using Microsoft.PSharp.Actors.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PSharp.Actors
{
    public class ActorExecutorMachine : Machine
    {
        #region states
        [Start]
        [OnEventDoAction(typeof(ActorMachine.ActorEvent), nameof(OnActorEvent))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnActorEvent()
        {
            var e = (this.ReceivedEvent as ActorMachine.ActorEvent);

            ActorModel.Runtime.Log($"<ActorModelLog> Machine '{base.Id}' is invoking '{e.MethodName}'.");

            MethodInfo mi = e.MethodClass.GetMethod(e.MethodName);
            try
            {
                object result = mi.Invoke(e.ClassInstance, e.Parameters);
                this.Send(e.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(result));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(Environment.ExitCode);
            }
        }
        #endregion
    }
}
