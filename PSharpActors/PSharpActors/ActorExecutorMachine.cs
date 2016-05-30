//-----------------------------------------------------------------------
// <copyright file="ActorExecutorMachine.cs">
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

using System.Reflection;

using Microsoft.PSharp.Actors.Bridge;

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// A P# actor executor machine.
    /// </summary>
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
            catch (TargetInvocationException ex)
            {
                throw new ActorModelException(ex.ToString());
            }
        }

        #endregion
    }
}
