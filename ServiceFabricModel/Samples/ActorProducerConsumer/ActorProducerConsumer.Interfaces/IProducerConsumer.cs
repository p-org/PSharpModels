using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorProducerConsumer.Interfaces
{
    public abstract class WorkItem
    {  }

    public class ActualWork : WorkItem
    {
        public int val;
        public ActualWork(int val)
        {
            this.val = val;
        }
    }

    public class WorkCompleted : WorkItem { }
    public class WorkNotAvaliable : WorkItem { }

    public interface ISharedState : IActor
    {
        // insert a value
        Task Insert(WorkItem val);

        // Retrive the head value
        Task<WorkItem> TryGet();
    }

    public interface IProducer : IActor
    {
        Task Produce(ISharedState state);
    }

    public interface IConsumer : IActor
    {
        Task SetBound(int bound);
        Task Consume(ISharedState state);
    }
}
