//-----------------------------------------------------------------------
// <copyright file="Grain.cs">
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
using System.Linq;
using System.Threading.Tasks;

using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.PSharp.Actors.Bridge;

using Orleans.Core;
using Orleans.Runtime;

using OrleansModel;
using System.Collections.Generic;
using Orleans.Streams;
using Orleans.Streams.Providers;

namespace Orleans
{
    /// <summary>
    /// The abstract base class for all grain classes.
    /// </summary>
    public abstract class Grain : IAddressable
    {
        #region fields
        
        /// <summary>
        /// The grain identity.
        /// </summary>
        internal IGrainIdentity Identity { get; private set; }

        /// <summary>
        /// The grain runtime.
        /// </summary>
        internal IGrainRuntime Runtime { get; private set; }

        /// <summary>
        /// The primary key.
        /// </summary>
        internal Guid PrimaryKey;

        /// <summary>
        /// The grain factory.
        /// </summary>
        protected IGrainFactory GrainFactory
        {
            get { return GrainClient.GrainFactory; }
        }

        /// <summary>
        /// The service factory.
        /// </summary>
        protected IServiceProvider ServiceProvider
        {
            get { return Runtime.ServiceProvider; }
        }

        private static MachineId StreamProviderDictionaryMachineId;
        #endregion

        #region constructors

        /// <summary>
        /// Constructor. This constructor should never be invoked. We
        /// expose it so that client code (subclasses of Grain) do not
        /// have to add a constructor. Client code should use the
        /// GrainFactory property to get a reference to a Grain.
        /// </summary>
        protected Grain()
        {
            if(StreamProviderDictionaryMachineId == null)
            {
                StreamProviderDictionaryMachineId = ActorModel.Runtime.CreateMachine(
                    typeof(StreamProviderDictionaryMachine), "StreamProviderDictionaryMachine");
            }               
        }

        /// <summary>
        /// Constructor. Grain implementers do not have to expose this
        /// constructor but can choose to do so. This constructor is
        /// particularly useful for unit testing where test code can
        /// create a Grain and replace the IGrainIdentity and IGrainRuntime
        /// with test doubles.
        /// </summary>
        /// <param name="identity">IGrainIdentity</param>
        /// <param name="runtime">IGrainRuntime</param>
        protected Grain(IGrainIdentity identity, IGrainRuntime runtime)
        {
            this.Identity = identity;
            this.Runtime = runtime;
            if (StreamProviderDictionaryMachineId == null)
            {
                StreamProviderDictionaryMachineId = ActorModel.Runtime.CreateMachine(
                    typeof(StreamProviderDictionaryMachine), "StreamProviderDictionaryMachine");
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// This method is called at the end of the process of
        /// activating a grain. It is called before any messages
        /// have been dispatched to the grain. For grains with
        /// declared persistent state, this method is called
        /// after the State property has been populated.
        /// </summary>
        /// <returns>Task</returns>
        public virtual Task OnActivateAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// This method is called at the begining of the
        /// process of deactivating a grain.
        /// </summary>
        /// <returns>Task</returns>
        public virtual Task OnDeactivateAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Registers a timer to send periodic callbacks to this grain.
        /// </summary>
        /// <param name="asyncCallback">Callback</param>
        /// <param name="state">State</param>
        /// <param name="dueTime">Timeout</param>
        /// <param name="period">Period</param>
        /// <returns>IDisposable</returns>
        protected virtual IDisposable RegisterTimer(Func<object, Task> asyncCallback,
            object state, TimeSpan dueTime, TimeSpan period)
        {
            MachineId timerMachine = ActorModel.Runtime.CreateMachine(typeof(TimerMachine),
                asyncCallback.Method.Name,
                new TimerMachine.InitEvent(ActorModel.Runtime.GetCurrentMachineId(),
                asyncCallback, state));
            return new TimerCancellationSource(ActorModel.Runtime.GetCurrentMachineId(), timerMachine);
        }

        /// <summary>
        /// Registers a reminder to send periodic callbacks to this grain.
        /// </summary>
        /// <param name="reminderName">Name</param>
        /// <param name="dueTime">Timeout</param>
        /// <param name="period">Period</param>
        /// <returns>Task<IGrainReminder></returns>
        protected virtual Task<IGrainReminder> RegisterOrUpdateReminder(string reminderName,
            TimeSpan dueTime, TimeSpan period)
        {
            var reminders = ActorModel.GetReminders(ActorModel.Runtime.GetCurrentMachineId());
            var reminder = reminders.SingleOrDefault(val => ((GrainReminder)val).ReminderName.Equals(reminderName));

            var task = new ActorCompletionTask<IGrainReminder>();
            var actorCompletionMachine = task.ActorCompletionMachine;
            if (reminder != null)
            {
                ActorModel.Runtime.SendEvent(actorCompletionMachine,
                    new ActorCompletionMachine.SetResultRequest(reminder));
            }
            else
            {
                ActorModel.Runtime.CreateMachine(typeof(GrainReminderMachine), reminderName,
                    new ReminderMachine.InitEvent(ActorModel.Runtime.GetCurrentMachineId(),
                    actorCompletionMachine, reminderName, null));
            }
            
            return task;
        }

        /// <summary>
        /// Unregisters a reminder.
        /// </summary>
        /// <param name="reminder">IGrainReminder</param>
        /// <returns>Task</returns>
        protected virtual Task UnregisterReminder(IGrainReminder reminder)
        {
            var reminders = ActorModel.GetReminders(ActorModel.Runtime.GetCurrentMachineId());
            var reminderToBeRemoved = reminders.SingleOrDefault(val
                => ((GrainReminder)val).ReminderName.Equals(reminder.ReminderName));
            if (reminderToBeRemoved != null)
            {
                reminderToBeRemoved.Dispose();
            }

            var task = new ActorCompletionTask<object>();
            var actorCompletionMachine = task.ActorCompletionMachine;
            ActorModel.Runtime.SendEvent(actorCompletionMachine,
                new ActorCompletionMachine.SetResultRequest(true));
            return task;
        }

        /// <summary>
        /// Returns the reminder which has the name "reminderName"
        /// </summary>
        /// <param name="reminderName">string</param>
        /// <returns>Task<IGrainReminder></returns>
        protected virtual Task<IGrainReminder> GetReminder(string reminderName)
        {
            var reminders = ActorModel.GetReminders(ActorModel.Runtime.GetCurrentMachineId());
            var reminder = reminders.SingleOrDefault(val => ((GrainReminder)val).ReminderName.Equals(reminderName));

            var task = new ActorCompletionTask<IGrainReminder>();
            var actorCompletionMachine = task.ActorCompletionMachine;
            ActorModel.Runtime.SendEvent(actorCompletionMachine,
                new ActorCompletionMachine.SetResultRequest(reminder));
            return task;
        }

        /// <summary>
        /// Returns all the reminders of this grain
        /// </summary>
        /// <returns>Task<List<IGrainReminder>></returns>
        protected virtual Task<List<IGrainReminder>> GetReminders()
        {
            var reminders = new List<IGrainReminder>();
            foreach (var reminder in ActorModel.GetReminders(ActorModel.Runtime.GetCurrentMachineId()))
            {
                reminders.Add(reminder as IGrainReminder);
            }

            var task = new ActorCompletionTask<List<IGrainReminder>>();
            var actorCompletionMachine = task.ActorCompletionMachine;
            ActorModel.Runtime.SendEvent(actorCompletionMachine,
                new ActorCompletionMachine.SetResultRequest(reminders));
            return task;
        }

        protected virtual IStreamProvider GetStreamProvider(string name)
        {
            ActorModel.Runtime.SendEvent(StreamProviderDictionaryMachineId, 
                new StreamProviderDictionaryMachine.EGetStreamProvider(name, ActorModel.Runtime.GetCurrentMachineId()));
            Event resultEvent = ActorModel.Runtime.Receive(typeof(StreamProviderDictionaryMachine.EStreamProvider));
            return ((StreamProviderDictionaryMachine.EStreamProvider)resultEvent).streamProvider;
        }
        #endregion
    }

    /// <summary>
    /// The abstract base class for all grain classes
    /// with declared persistent state.
    /// </summary>
    public class Grain<TGrainState> : Grain, IStatefulGrain
    {
        /// <summary>
        /// The grain state.
        /// </summary>
        //private readonly GrainState<TGrainState> GrainState;
        internal GrainState<TGrainState> GrainState;

        IGrainState IStatefulGrain.GrainState
        {
            get
            {
                return this.GrainState;
            }
        }

        /// <summary>
        /// The grain storage.
        /// </summary>
        private IStorage Storage;

        /// <summary>
        /// The grain state.
        /// </summary>
        protected TGrainState State
        {
            get { return this.GrainState.State; }
            set { this.GrainState.State = value; }
        }

        /// <summary>
        /// Constructor. This constructor should never be invoked. We
        /// expose it so that client code (subclasses of Grain) do not
        /// have to add a constructor. Client code should use the
        /// GrainFactory property to get a reference to a Grain.
        /// </summary>
        protected Grain()
            : base()
        {
            this.GrainState = new GrainState<TGrainState>();
        }

        /// <summary>
        /// Constructor. Grain implementers do not have to expose this
        /// constructor but can choose to do so. This constructor is
        /// particularly useful for unit testing where test code can
        /// create a Grain and replace the IGrainIdentity and IGrainRuntime
        /// with test doubles.
        /// </summary>
        /// <param name="identity">IGrainIdentity</param>
        /// <param name="runtime">IGrainRuntime</param>
        /// <param name="state">TGrainState</param>
        /// <param name="storage">IStorage</param>
        protected Grain(IGrainIdentity identity, IGrainRuntime runtime,
            TGrainState state, IStorage storage)
            : base(identity, runtime)
        {
            this.GrainState = new GrainState<TGrainState>(state);
            this.Storage = storage;
        }

        /// <summary>
        /// Sets the storage of the grain to the
        /// specified storage.
        /// </summary>
        /// <param name="storage">IStorage</param>
        void IStatefulGrain.SetStorage(IStorage storage)
        {
            this.Storage = storage;
        }

        /// <summary>
        /// Async method to cause the current grain state data
        /// to be cleared and reset.  This will usually mean
        /// the state record is deleted from backing store, but
        /// the specific behavior is defined by the storage
        /// provider instance configured for this grain.
        /// </summary>
        /// <returns>Task</returns>
        protected virtual Task ClearStateAsync()
        {
            return this.Storage.ClearStateAsync();
        }

        /// <summary>
        /// Async method to cause write of the current
        /// grain state data into backing store.
        /// </summary>
        /// <returns></returns>
        protected virtual Task WriteStateAsync()
        {
            return this.Storage.WriteStateAsync();
        }

        /// <summary>
        /// Async method to cause refresh of the current grain
        /// state data from backing store. Any previous contents
        /// of the grain state data will be overwritten.
        /// </summary>
        /// <returns></returns>
        protected virtual Task ReadStateAsync()
        {
            return this.Storage.ReadStateAsync();
        }
    }

    /// <summary>
    /// Machine that that provides access to StreamProviderDictionary(stream provider name -> IStreamProvider).
    /// Required to handle concurrent accesses to this dictionary.
    /// </summary>
    class StreamProviderDictionaryMachine : Machine
    {
        #region events
        public class EGetStreamProvider : Event
        {
            public string Name;
            public MachineId Target;
            public EGetStreamProvider(string name, MachineId target)
            {
                this.Name = name;
                this.Target = target;
            }
        }

        public class EStreamProvider : Event
        {
            public IStreamProvider streamProvider;

            public EStreamProvider(IStreamProvider streamProvider)
            {
                this.streamProvider = streamProvider;
            }
        }
        #endregion

        #region fields
        private IDictionary<string, IStreamProvider> StreamProviderDictionary;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInit))]
        [OnEventDoAction(typeof(EGetStreamProvider), nameof(OnGetStreamProvider))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnInit()
        {
            StreamProviderDictionary = new Dictionary<string, IStreamProvider>();
        }

        void OnGetStreamProvider()
        {
            var e = ReceivedEvent as EGetStreamProvider;
            if (StreamProviderDictionary.ContainsKey(e.Name))
            {
                Send(e.Target, new EStreamProvider(StreamProviderDictionary[e.Name]));
            }
            else
            {
                IStreamProvider streamProvider = new StreamProvider(e.Name, false);
                StreamProviderDictionary.Add(e.Name, streamProvider);
                Send(e.Target, new EStreamProvider(streamProvider));
            }
        }
        #endregion
    }
}
