//-----------------------------------------------------------------------
// <copyright file="ActorModel.cs">
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

using Microsoft.PSharp.Actors.Bridge;

namespace Microsoft.PSharp.Actors
{
    /// <summary>
    /// The P# actor model.
    /// </summary>
    public static class ActorModel
    {
        #region fields

        /// <summary>
        /// The P# runtime.
        /// </summary>
        public static PSharpRuntime Runtime { get; private set; }

        #endregion

        #region methods

        /// <summary>
        /// Starts executing the specified action using the
        /// specified P# runtime.
        /// </summary>
        /// <param name="runtime">PSharpRuntime</param>
        public static void Start(PSharpRuntime runtime, Action action)
        {
            if (runtime == null)
            {
                throw new InvalidOperationException(
                    "The P# runtime has not been initialized.");
            }

            ActorModel.Runtime = runtime;
            ActorModel.Runtime.CreateMachine(typeof(ActorRootMachine),
                new ActorRootMachine.RunEvent(action));
        }

        /// <summary>
        /// Gets the result of the specified task.
        /// </summary>
        /// <typeparam name="TResult">TResult</typeparam>
        /// <param name="task">Task</param>
        /// <returns>TResult</returns>
        public static TResult GetResult<TResult>(Task<TResult> task)
        {
            return ((ActorCompletionTask<TResult>)task).Result;
        }

        /// <summary>
        /// Waits for the specified task to complete.
        /// </summary>
        /// <param name="task">Task</param>
        public static void Wait(Task task)
        {
            ((ActorCompletionTask<object>)task).Wait();
        }

        /// <summary>
        /// Waits for the specified task to complete.
        /// </summary>
        /// <param name="task">Task</param>
        public static void Wait<TResult>(Task<TResult> task)
        {
            ((ActorCompletionTask<TResult>)task).Wait();
        }

        #endregion
    }
}
