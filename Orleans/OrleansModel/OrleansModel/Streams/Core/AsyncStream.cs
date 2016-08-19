using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Streams.Core
{
    /// <summary>
    /// Models IAsyncStream APIs. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AsyncStream<T> : IAsyncStream<T>
    {
        public MachineId StreamMachineId;
        private Guid streamGuid;


        public AsyncStream()
        {
            streamGuid = new Guid();
            StreamMachineId = ActorModel.Runtime.CreateMachine(typeof(StreamMachine));
        }
        public Guid Guid
        {
            get
            {
                return streamGuid;
            }
        }

        public bool IsRewindable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Namespace
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ProviderName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int CompareTo(IAsyncStream<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IAsyncStream<T> other)
        {
            throw new NotImplementedException();
        }

        public Task<IList<StreamSubscriptionHandle<T>>> GetAllSubscriptionHandles()
        {
            throw new NotImplementedException();
        }

        public Task OnCompletedAsync()
        {
            throw new NotImplementedException();
        }

        public Task OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task OnNextAsync(T item, StreamSequenceToken token = null)
        {
            ActorModel.Runtime.SendEvent(StreamMachineId, new StreamMachine.AddDataToQueue(item));
            return Task.FromResult(true);
        }

        public Task OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken token = null)
        {
            throw new NotImplementedException();
        }

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer)
        {
            ActorModel.Runtime.SendEvent(StreamMachineId, new StreamMachine.Subscribe(ActorModel.Runtime.GetCurrentMachineId(), observer));
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// P# machine to handle stream events.
    /// </summary>
    class StreamMachine : Machine
    {
        #region events
        public class AddDataToQueue : Event
        {
            public object Item;
            public AddDataToQueue(object item)
            {
                this.Item = item;
            }
        }

        public class Subscribe : Event
        {
            public MachineId TargetGrain;
            public object Observer;

            public Subscribe(MachineId targetGrain, object observer)
            {
                this.TargetGrain = targetGrain;
                this.Observer = observer;
            }
        }
        #endregion

        #region fields
        Queue<object> itemQueue;
        Dictionary<MachineId, object> grainToObserverMap;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInit))]
        [OnEventDoAction(typeof(AddDataToQueue), nameof(OnAddDataToQueue))]
        [OnEventDoAction(typeof(Subscribe), nameof(OnSubscribe))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnInit()
        {
            itemQueue = new Queue<object>();
            grainToObserverMap = new Dictionary<MachineId, object>();
        }

        void OnAddDataToQueue()
        {
            var receivedEvent = ReceivedEvent as AddDataToQueue;
            itemQueue.Enqueue(receivedEvent.Item);
        }

        void OnSubscribe()
        {
            var receivedEvent = ReceivedEvent as Subscribe;
            grainToObserverMap.Add(receivedEvent.TargetGrain, receivedEvent.Observer);
        }
        #endregion
    }
}
