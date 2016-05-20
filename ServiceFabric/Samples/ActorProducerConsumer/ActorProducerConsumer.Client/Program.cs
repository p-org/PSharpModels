using ActorProducerConsumer.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorProducerConsumer.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var producer = ActorProxy.Create<IProducer>(ActorId.NewId(), "PC");
            var consumer = ActorProxy.Create<IConsumer>(ActorId.NewId(), "PC");
            var state = ActorProxy.Create<ISharedState>(ActorId.NewId(), "PC");
            
            consumer.SetBound(2).Wait();
            var t1 = producer.Produce(state);
            var t2 = consumer.Consume(state);
            Task.WaitAll(t1, t2);
            
            //Console.ReadLine();
        }
    }
}
