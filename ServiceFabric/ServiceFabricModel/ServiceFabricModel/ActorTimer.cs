using Microsoft.PSharp;
using Microsoft.PSharp.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    class ActorTimer : TimerCancellationSource, IActorTimer
    {
        public ActorTimer(TimeSpan dueTime, TimeSpan period, MachineId actor, MachineId timer)
            :base(actor, timer)
        {

        }

        public TimeSpan DueTime
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public TimeSpan Period
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
