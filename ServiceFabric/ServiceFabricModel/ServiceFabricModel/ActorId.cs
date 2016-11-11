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

using Microsoft.PSharp.Actors;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorId : IEquatable<ActorId>
    {
        #region fields

        /// <summary>
        /// The identity counter.
        /// </summary>
        private static long IdentityCounter = 0;

        /// <summary>
        /// The unique id of the actor.
        /// </summary>
        public readonly long Id;

        /// <summary>
        /// The unique id of the actor specified as a Guid.
        /// </summary>
        public readonly Guid guId;

        /// <summary>
        /// The unique id of the actor specified as a String.
        /// </summary>
        public readonly String strId;
        /// <summary>
        /// true when ActorId constructed with Guid.
        /// </summary>
        private bool isGuid;
        private bool isString;

        #endregion

        #region constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ActorId()
        {
            ActorModel.RegisterCleanUpAction(() =>
            {
                ActorId.IdentityCounter = 0;
            });
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Id</param>
        public ActorId(long id)
        {
            this.Id = id;
            isGuid = false;
            isString = false;
        }

        public ActorId(Guid guid)
        {
            this.guId = guid;
            isGuid = true;
            isString = false;
        }

        public ActorId(String strId)
        {
            this.strId = strId;
            isGuid = false;
            isString = true;
        }
        #endregion

        #region methods

        /// <summary>
        /// Creates a new actor id.
        /// </summary>
        /// <returns>ActorId</returns>
        public static ActorId NewId()
        {
            ActorId.IdentityCounter++;
            return (new ActorId(IdentityCounter));
        }

        /// <summary>
        /// Gets the id for this ActorId.
        /// </summary>
        /// <returns>Id</returns>
        public long GetLongId()
        {
            return (long)this.Id;
        }

        public bool Equals(ActorId other)
        {
            if (!isGuid && !isString)
                return (this.Id == other.Id);
            else 
                return (this.guId.Equals(other.guId));
        }

        public override int GetHashCode()
        {
            if (this.isGuid)
                return this.guId.GetHashCode();
            else if (this.isString)
                return this.strId.GetHashCode();
            else
                return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }

        #endregion
    }
}
