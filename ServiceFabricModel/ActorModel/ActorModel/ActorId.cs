using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorId : IEquatable<ActorId>
    {
        static long Identity = 0;
        public readonly long Id;
        public ActorId(long id)
        {
            Id = id;
        }

        public static ActorId NewId()
        {
            Identity++;
            return (new ActorId(Identity));
        }

        public bool Equals(ActorId other)
        {
            return (this.Id == other.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }

        //
        // Summary:
        //     Gets id for ActorId whose Microsoft.ServiceFabric.Actors.ActorIdKind is Microsoft.ServiceFabric.Actors.ActorIdKind.Long.
        //
        // Returns:
        //     System.Int64 id value for AcotrId.
        public long GetLongId()
        {
            return (long)this.Id;
        }

        public static ActorId CreateRandom()
        {
            long randIdentity = new Random(7).Next();
            return new ActorId(randIdentity);
        }
    }
}
