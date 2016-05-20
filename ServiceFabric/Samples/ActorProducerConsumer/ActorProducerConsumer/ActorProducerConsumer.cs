using ActorModel;
using ActorProducerConsumer.Interfaces;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorProducerConsumer
{
    public class ActorSharedState : Actor, ISharedState
    {
        private Queue<WorkItem> q = new Queue<WorkItem>();
          
           
        protected override Task OnActivateAsync()
        {
            var cq = this.StateManager.TryGetStateAsync<Queue<WorkItem>>("state").Result;
            if (cq.HasValue)
                q = cq.Value;
            return base.OnActivateAsync();
        }
       

        protected override async Task OnDeactivateAsync()
        {
            await this.StateManager.SetStateAsync("state", q);
            await base.OnDeactivateAsync();
        }

        // insert a value
        public Task Insert(WorkItem val)
        {
            q.Enqueue(val);
            return Task.FromResult(true);
        }

        // Retrive the head value
        public Task<WorkItem> TryGet()
        {
            if (q.Count == 0)
                return Task.FromResult(new WorkNotAvaliable() as WorkItem);
            return Task.FromResult(q.Dequeue());
        }
    }
    
    public class ActorProducer : Actor, IProducer
    {
        public async Task Produce(ISharedState state)
        {
            await state.Insert(new ActualWork(0));
            Console.WriteLine("Produced: 0");

            await state.Insert(new ActualWork(1));
            Console.WriteLine("Produced: 1");

            await state.Insert(new WorkCompleted());
            Console.WriteLine("Produced: Completed");
        }
    }

    public class ActorConsumer : Actor, IConsumer
    {
        int bound = 0;
        public Task SetBound(int bound)
        {
            this.bound = bound;
            return Task.FromResult(true);
        }

        public async Task Consume(ISharedState state)
        {
            WorkItem wi = null;
            int count = 0;
            do
            {
                do
                {
                    wi = await state.TryGet();
                } while (wi is WorkNotAvaliable);

                if (wi is WorkCompleted) break;

                var awi = wi as ActualWork;
                Console.WriteLine("Consumed work: {0}", awi.val);
                count++;
                if (count > bound) throw new Exception();
                //Debug.Assert(count < bound);
            } while (true);
        }
    }

}
