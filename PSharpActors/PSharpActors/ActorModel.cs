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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        /// <summary>
        /// The configuration.
        /// </summary>
        internal static Configuration Configuration { get; private set; }

        /// <summary>
        /// Map from actors to their reentrant action handlers.
        /// </summary>
        private static IDictionary<MachineId, Action<ActorMachine.ActorEvent>> ReentrantActionHandlers;

        /// <summary>
        /// Map from Machine ID to the latest calling context.
        /// </summary>
        internal static IDictionary<MachineId, List<MachineId>> ExecutionContext;

        /// <summary>
        /// Map of actors to a boolean specifying if the actor is
        /// reentrant or not.
        /// </summary>
        internal static IDictionary<MachineId, bool> ReentrantActors;

        /// <summary>
        /// Map from actors to sets of registered timers.
        /// </summary>
        internal static IDictionary<MachineId, ISet<MachineId>> RegisteredTimers;

        /// <summary>
        /// Map from actors to sets of registered reminders.
        /// </summary>
        internal static IDictionary<MachineId, ISet<ReminderCancellationSource>> RegisteredReminders;

        /// <summary>
        /// Set of cleanup actions to perform in each iteration.
        /// </summary>
        private static ISet<Action> CleanUpActions;

        #endregion

        #region constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ActorModel()
        {
            ActorModel.Configuration = Configuration.Default();

            ActorModel.ReentrantActors = new ConcurrentDictionary<MachineId, bool>();
            ActorModel.ReentrantActionHandlers = new ConcurrentDictionary<MachineId, Action<ActorMachine.ActorEvent>>();
            ActorModel.RegisteredTimers = new ConcurrentDictionary<MachineId, ISet<MachineId>>();
            ActorModel.RegisteredReminders = new ConcurrentDictionary<MachineId, ISet<ReminderCancellationSource>>();
            ActorModel.ExecutionContext = new ConcurrentDictionary<MachineId, List<MachineId>>();

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (string file in Directory.GetFiles(path, "*_PSharpProxy.dll"))
            {
                File.Delete(file);
            }

            ActorModel.CleanUpActions = new HashSet<Action>();
        }

        #endregion

        #region methods

        /// <summary>
        /// Configures the P# actor model with the specified configuration.
        /// </summary>
        /// <param name="config">Configuration</param>
        public static void Configure(Configuration config)
        {
            ActorModel.Configuration = config;
        }

        /// <summary>
        /// Starts executing the specified action using the
        /// specified P# runtime.
        /// </summary>
        /// <param name="runtime">PSharpRuntime</param>
        /// <param name="action">Action</param>
        public static void Start(PSharpRuntime runtime, Action action)
        {
            if (runtime == null)
            {
                throw new InvalidOperationException(
                    "The P# runtime has not been initialized.");
            }

            foreach (var cleanupAction in ActorModel.CleanUpActions)
            {
                cleanupAction();
            }

            ActorModel.Runtime = runtime;
            ActorModel.ReentrantActionHandlers.Clear();
            ActorModel.ReentrantActors.Clear();
            ActorModel.RegisteredTimers.Clear();
            ActorModel.RegisteredReminders.Clear();
            ActorModel.ExecutionContext.Clear();

            ActorModel.Runtime.CreateMachine(typeof(ActorRootMachine),
                new ActorRootMachine.RunEvent(action));
        }

        /// <summary>
        /// Checks if the assertion holds, and if not it reports
        /// an error and exits.
        /// </summary>
        /// <param name="predicate">Predicate</param>
        public static void Assert(bool predicate)
        {
            ActorModel.Runtime.Assert(predicate);
        }

        /// <summary>
        /// Checks if the assertion holds, and if not it reports
        /// an error and exits.
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="s">Message</param>
        /// <param name="args">Message arguments</param>
        public static void Assert(bool predicate, string s, params object[] args)
        {
            ActorModel.Runtime.Assert(predicate, s, args);
        }

        /// <summary>
        /// Returns all reminder cancellation sources
        /// associated with the registered reminders
        /// for the specified P# machine id.
        /// </summary>
        /// <returns>ReminderCancellationSources</returns>
        public static ISet<ReminderCancellationSource> GetReminders(MachineId mid)
        {
            if (!ActorModel.RegisteredReminders.ContainsKey(mid))
            {
                return new HashSet<ReminderCancellationSource>();
            }

            return ActorModel.RegisteredReminders[mid];
        }

        /// <summary>
        /// Returns a nondeterministic boolean choice, that
        /// can be controlled during analysis or testing.
        /// </summary>
        /// <returns></returns>
        public static bool Random()
        {
            return ActorModel.Runtime.Random();
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

        /// <summary>
        /// Logs the specified text.
        /// </summary>
        /// <param name="s">Text</param>
        /// <param name="args">Arguments</param>
        public static void Log(string s, params object[] args)
        {
            ActorModel.Runtime.Log(s, args);
        }

        /// <summary>
        /// Registers a cleanup action, to execute at the
        /// beginning of each testing iteration. Any static
        /// fields should be reset in this action.
        /// </summary>
        /// <param name="action">Action</param>
        public static void RegisterCleanUpAction(Action action)
        {
            ActorModel.CleanUpActions.Add(action);
        }

        /// <summary>
        /// Registers the reentrant action handler for the
        /// specified actor machine.
        /// </summary>
        /// <param name="machine">MachineId</param>
        /// <param name="handler">Action</param>
        internal static void RegisterActionHandler(MachineId machine,
            Action<ActorMachine.ActorEvent> handler)
        {
            ActorModel.ReentrantActionHandlers.Add(machine, handler);
        }

        /// <summary>
        /// Returns an actor handler, for handling reentrant events.
        /// </summary>
        /// <param name="machine">MachineId</param>
        /// <returns>Action</returns>
        internal static Action<ActorMachine.ActorEvent> GetReentrantActionHandler(MachineId machine)
        {
            ActorModel.Assert(ActorModel.ReentrantActors[machine],
                $"{machine.Name} is not reentrant.");
            return ActorModel.ReentrantActionHandlers[machine];
        }

        #endregion
    }
}
