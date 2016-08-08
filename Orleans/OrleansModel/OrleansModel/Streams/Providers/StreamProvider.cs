using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using Orleans.Streams.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Streams.Providers
{
    /// <summary>
    /// Stream provider class.
    /// </summary>
    class StreamProvider : IStreamProvider
    {
        private string name;
        private bool isRewindable;

        private static MachineId StreamDictionaryMachineID;

        public StreamProvider(string name, bool isRewindable)
        {
            this.name = name;
            this.isRewindable = isRewindable;
            if(StreamDictionaryMachineID == null)
            {
                StreamDictionaryMachineID = ActorModel.Runtime.CreateMachine(typeof(StreamDictionaryMachine));
            }
        }

        public bool IsRewindable
        {
            get
            {
                return this.isRewindable;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public IAsyncStream<T> GetStream<T>(Guid streamId, string streamNamespace)
        {
            ActorModel.Runtime.SendEvent(StreamDictionaryMachineID, new StreamDictionaryMachine.EGetStream(
                streamId, streamNamespace, ActorModel.Runtime.GetCurrentMachineId()));
            Event resultEvent = ActorModel.Runtime.Receive(typeof(StreamDictionaryMachine.EStream));
            object resultStream = ((StreamDictionaryMachine.EStream)resultEvent).stream;
            if (resultStream != null)
            {
                return (IAsyncStream<T>)((StreamDictionaryMachine.EStream)resultEvent).stream;
            }
            else
            {
                object streamToAdd = new AsyncStream<T>();
                ActorModel.Runtime.SendEvent(StreamDictionaryMachineID, 
                    new StreamDictionaryMachine.EAddStream(streamId, streamNamespace, streamToAdd, ActorModel.Runtime.GetCurrentMachineId()));
                ActorModel.Runtime.Receive(typeof(StreamDictionaryMachine.EStream));
                object addedStream = ((StreamDictionaryMachine.EStream)resultEvent).stream;
                return (IAsyncStream<T>)addedStream;
            }
        }
    }

    /// <summary>
    /// Machine that that provides access to StreamDictionary(<ID, name> -> IAsyncStream).
    /// Part of StreamProvider. Required to handle concurrent accesses to StreamDictionary.
    /// </summary>
    class StreamDictionaryMachine : Machine
    {
        #region events
        public class EGetStream : Event
        {
            public Guid Id;
            public string Name;
            public MachineId Target;
            public EGetStream(Guid id, string name, MachineId target)
            {
                this.Id = id;
                this.Name = name;
                this.Target = target;
            }
        }

        public class EStream : Event
        {
            public object stream;

            public EStream(object stream)
            {
                this.stream = stream;
            }
        }

        public class EAddStream : Event
        {
            public Guid streamId;
            public string streamNamespace;
            public object streamToAdd;
            public MachineId Target;

            public EAddStream(Guid streamId, string streamNamespace, object streamToAdd, MachineId Target)
            {
                this.streamId = streamId;
                this.streamNamespace = streamNamespace;
                this.streamToAdd = streamToAdd;
                this.Target = Target;
            }
        }
        #endregion

        #region fields
        private IDictionary<Tuple<Guid, string>, object> StreamDictionary;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInit))]
        [OnEventDoAction(typeof(EGetStream), nameof(OnGetStream))]
        [OnEventDoAction(typeof(EAddStream), nameof(OnAddStream))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnInit()
        {
            StreamDictionary = new Dictionary<Tuple<Guid, string>, object>();
        }

        void OnGetStream()
        {
            var e = ReceivedEvent as EGetStream;
            Tuple<Guid, string> checkKey = new Tuple<Guid, string>(e.Id, e.Name);
            if (StreamDictionary.ContainsKey(checkKey))
            {
                Send(e.Target, new EStream(StreamDictionary[checkKey]));
            }
            else
            {
                Send(e.Target, new EStream(null));
            }
        }

        void OnAddStream()
        {
            var e = ReceivedEvent as EAddStream;
            Tuple<Guid, string> checkKey = new Tuple<Guid, string>(e.streamId, e.streamNamespace);
            if (StreamDictionary.ContainsKey(checkKey))
            {
                Send(e.Target, new EStream(StreamDictionary[checkKey]));
            }
            else
            {
                StreamDictionary.Add(checkKey, e.streamToAdd);
                Send(e.Target, new EStream(e.streamToAdd));
            }
        }
        #endregion
    }
}
