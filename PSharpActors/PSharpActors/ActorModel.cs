﻿//-----------------------------------------------------------------------
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
        /// Map from machine ids to actor machines.
        /// </summary>
        internal static IDictionary<MachineId, ActorMachine> ActorMachineMap;

        /// <summary>
        /// Map of actors to a boolean specifying if the actor is
        /// reentrant or not.
        /// </summary>
        internal static IDictionary<MachineId, bool> ReentrantActors;

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

            ActorModel.ActorMachineMap = new ConcurrentDictionary<MachineId, ActorMachine>();
            ActorModel.ReentrantActors = new ConcurrentDictionary<MachineId, bool>();

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

            ActorModel.Runtime = runtime;
            ActorModel.ActorMachineMap.Clear();
            ActorModel.ReentrantActors.Clear();

            ActorModel.Runtime.CreateMachine(typeof(ActorRootMachine),
                new ActorRootMachine.Configure(action, CleanUpActions));
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
            return ActorModel.ActorMachineMap[mid].RegisteredReminders;
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
            if (task is ActorCompletionTask<TResult>)
            {
                return ((ActorCompletionTask<TResult>)task).Result;
            }
            else
            {
                return task.Result;
            }
        }

        /// <summary>
        /// Waits for the specified task to complete.
        /// </summary>
        /// <param name="task">Task</param>
        public static void Wait(Task task)
        {
            if (task is ActorCompletionTask<object>)
            {
                ((ActorCompletionTask<object>)task).Wait();
            }
            else
            {
                task.Wait();
            }
        }

        /// <summary>
        /// Waits for the specified task to complete.
        /// </summary>
        /// <param name="task">Task</param>
        public static void Wait<TResult>(Task<TResult> task)
        {
            if (task is ActorCompletionTask<TResult>)
            {
                ((ActorCompletionTask<TResult>)task).Wait();
            }
            else
            {
                task.Wait();
            }
        }

        /// <summary>
        /// Timestamp used with WhenAny
        /// </summary>
        public static int timestamp = 0;

        /// <summary>
        /// Waits for one of the tasks in the input list of tasks to complete.
        /// </summary>
        /// <typeparam name="TResult">TResult</typeparam>
        /// <param name="tasks">IEnumerable</param>
        /// <returns>Task</returns>
        public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            List<Task<TResult>> taskList = new List<Task<TResult>>(tasks);

            if (taskList.Count == 0)
            {
                var resultTask = new ActorCompletionTask<Task<TResult>>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(
                    new ActorCompletionTask<TResult>()));
                return resultTask;
            }
            if (taskList[0] is ActorCompletionTask<TResult>)
            {
                foreach (var task in taskList)
                {
                    MachineId mc = Runtime.CreateMachine(typeof(WaitMachine<TResult>), new WaitMachine<TResult>.CompleteTask(
                        (ActorCompletionTask<TResult>)task, Runtime.GetCurrentMachineId(), timestamp));
                }
                var receivedEvent = Runtime.Receive(typeof(WaitMachine<TResult>.TaskCompleted), new Func<Event, bool>(
                    e => ((WaitMachine<TResult>.TaskCompleted)e).Timestamp == timestamp));
                timestamp++;
                var resultTask = new ActorCompletionTask<Task<TResult>>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(
                    ((WaitMachine<TResult>.TaskCompleted)receivedEvent).ResultTask));
                return resultTask;
            }
            else
            {
                return Task.WhenAny(tasks);
            }
        }

        /// <summary>
        /// Waits for all the input tasks to complete
        /// </summary>
        /// <typeparam name="TResult">TResult</typeparam>
        /// <param name="tasks">IEnumerable</param>
        /// <returns>Task</returns>
        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            List<Task<TResult>> taskList = new List<Task<TResult>>(tasks);
            List<TResult> resultList = new List<TResult>();

            if (taskList.Count == 0)
            {
                var resultTask = new ActorCompletionTask<TResult[]>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(resultList.ToArray()));
                return resultTask;
            }
            if (taskList[0] is ActorCompletionTask<TResult>)
            {
                foreach (var task in taskList)
                {
                    resultList.Add(((ActorCompletionTask<TResult>)task).Result);
                }
                var resultTask = new ActorCompletionTask<TResult[]>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(resultList.ToArray()));
                return resultTask;
            }
            else
            {
                Console.WriteLine("CAUGHT!!!!!!!!");
                return Task.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Waits for all the input tasks to complete
        /// </summary>
        /// <param name="tasks">IEnumerable</param>
        /// <returns>Task</returns>
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
            List<Task> taskList = new List<Task>(tasks);

            if(taskList.Count == 0)
            {
                var resultTask = new ActorCompletionTask<object>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(true));
                return resultTask;
            }

            if (taskList[0] is ActorCompletionTask<object>)
            {
                foreach (var task in taskList)
                {
                    ((ActorCompletionTask<object>)task).Wait();
                }
                var resultTask = new ActorCompletionTask<object>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(true));
                return resultTask;
            }
            else
            {
                return Task.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Waits for all the input tasks to complete
        /// </summary>
        /// <typeparam name="TResult">TResult</typeparam>
        /// <param name="tasks">params Task<TResult>[]</param>
        /// <returns>Task</returns>
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
            List<TResult> resultList = new List<TResult>();

            if (tasks.Length == 0)
            {
                var resultTask = new ActorCompletionTask<TResult[]>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(resultList.ToArray()));
                return resultTask;
            }

            if (tasks[0] is ActorCompletionTask<TResult>)
            {
                foreach (var task in tasks)
                {
                    resultList.Add(((ActorCompletionTask<TResult>)task).Result);
                }
                var resultTask = new ActorCompletionTask<TResult[]>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(resultList.ToArray()));
                return resultTask;
            }
            else
            {
                return Task.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Waits for all the input tasks to complete
        /// </summary>
        /// <param name="tasks">Task[]</param>
        /// <returns>Task</returns>
        public static Task WhenAll(params Task[] tasks)
        {
            if (tasks.Length == 0)
            {
                var resultTask = new ActorCompletionTask<object>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(true));
                return resultTask;
            }

            if (tasks[0] is ActorCompletionTask<object>)
            {
                foreach (var task in tasks)
                {
                    ((ActorCompletionTask<object>)task).Wait();
                }
                var resultTask = new ActorCompletionTask<object>();
                Runtime.SendEvent(resultTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(true));
                return resultTask;
            }
            else
            {
                return Task.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Sends a message to halt the P# actor. Only used
        /// during testing with P#.
        /// </summary>
        /// <param name="actor">IPSharpActor</param>
        public static void Halt(IPSharpActor actor)
        {
            ActorModel.Runtime.Log($"<ActorModelLog> Halt invoked on '{actor.Id}'.");
            ActorModel.Runtime.SendEvent(actor.Id, new Halt());
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
        /// Returns an actor handler, for handling reentrant events.
        /// </summary>
        /// <param name="machine">MachineId</param>
        /// <returns>Action</returns>
        internal static Action<ActorMachine.ActorEvent> GetReentrantActionHandler(MachineId machine)
        {
            ActorModel.Assert(ActorModel.ReentrantActors[machine],
                $"{machine.Name} is not reentrant.");
            return ActorModel.ActorMachineMap[machine].ReentrantActionHandler;
        }
        #endregion
    }
}
