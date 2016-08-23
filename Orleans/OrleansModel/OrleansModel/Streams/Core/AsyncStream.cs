using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Microsoft.PSharp.Actors.Bridge;
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
        private Guid StreamGuid;
        private string StreamNamespace;
        private bool streamIsRewindable;

        public AsyncStream(Guid streamGuid, string streamNamespace)
        {
            this.StreamGuid = streamGuid;
            this.StreamNamespace = streamNamespace;
            this.streamIsRewindable = false;
            StreamMachineId = ActorModel.Runtime.CreateMachine(typeof(StreamMachine<T>));
        }
        public Guid Guid
        {
            get
            {
                return StreamGuid;
            }
        }

        public bool IsRewindable
        {
            get
            {
                return streamIsRewindable;
            }
        }

        public string Namespace
        {
            get
            {
                return StreamNamespace;
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
            ActorModel.Runtime.SendEvent(StreamMachineId, new StreamMachine<T>.PushToSubscribers<T>(item));
            return Task.FromResult(true);
        }

        public Task OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken token = null)
        {
            throw new NotImplementedException();
        }

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer)
        {
            ActorModel.Runtime.SendEvent(StreamMachineId, new StreamMachine<T>.Subscribe<T>(ActorModel.Runtime.GetCurrentMachineId(), observer));
            ActorCompletionTask<StreamSubscriptionHandle<T>> returnTask = new ActorCompletionTask<StreamSubscriptionHandle<T>>();
            ActorModel.Runtime.SendEvent(returnTask.ActorCompletionMachine, new ActorCompletionMachine.SetResultRequest(
                new StreamSubscriptionHandleImpl<T>(new StreamIdentity(StreamGuid, StreamNamespace))));
            return returnTask;
        }
    }

    /// <summary>
    /// P# machine to handle stream events.
    /// </summary>
    class StreamMachine<T> : Machine
    {
        #region events
        public class PushToSubscribers<T> : Event
        {
            public object Item;
            public PushToSubscribers(object item)
            {
                this.Item = item;
            }
        }

        public class Subscribe<T> : Event
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
        [OnEventDoAction(typeof(PushToSubscribers<>), nameof(OnPushToSubscribers))]
        [OnEventDoAction(typeof(Subscribe<>), nameof(OnSubscribe))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnInit()
        {
            itemQueue = new Queue<object>();
            grainToObserverMap = new Dictionary<MachineId, object>();
        }

        void OnPushToSubscribers()
        {
            var receivedEvent = ReceivedEvent as PushToSubscribers<T>;
            itemQueue.Enqueue(receivedEvent.Item);

            foreach(var key in grainToObserverMap.Keys)
            {
                Send(key, new ActorMachine.StreamEvent<T>(grainToObserverMap[key]));
            }
        }

        void OnSubscribe()
        {
            var receivedEvent = ReceivedEvent as Subscribe<T>;
            grainToObserverMap.Add(receivedEvent.TargetGrain, receivedEvent.Observer);
        }
        #endregion
    }
}
