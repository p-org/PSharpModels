//-----------------------------------------------------------------------
// <copyright file="ActorId.cs">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

using System;

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
