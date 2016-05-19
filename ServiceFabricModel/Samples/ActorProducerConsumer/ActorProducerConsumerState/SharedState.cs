using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ActorProducerConsumerState
{
    [DataContract]
    public class SharedState
    {
        [DataMember]
        public int Count;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "StatefulActorModelState[Count = {0}]", Count);
        }
    }
}
