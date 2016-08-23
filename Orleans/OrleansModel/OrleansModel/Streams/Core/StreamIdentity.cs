using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Streams.Core
{
    public class StreamIdentity : IStreamIdentity
    {
        Guid StreamGuid;
        string StreamNamespace;

        public StreamIdentity(Guid streamGuid, string streamNamespace)
        {
            this.StreamGuid = streamGuid;
            this.StreamNamespace = streamNamespace;
        }
        public Guid Guid
        {
            get
            {
                return StreamGuid;
            }
        }

        public string Namespace
        {
            get
            {
                return StreamNamespace;
            }
        }
    }
}
