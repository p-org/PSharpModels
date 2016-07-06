//-----------------------------------------------------------------------
// <copyright file="GrainId.cs">
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

using Orleans;
using Orleans.Core;

namespace OrleansModel
{
    /// <summary>
    /// Class implementing the id of a grain.
    /// </summary>
    internal class GrainId : IGrainIdentity
    {
        #region fields

        /// <summary>
        /// The primary key.
        /// </summary>
        public Guid PrimaryKey { get; private set; }

        /// <summary>
        /// The primary long key.
        /// </summary>
        public long PrimaryKeyLong { get; private set; }

        /// <summary>
        /// The primary string key.
        /// </summary>
        public string PrimaryKeyString { get; private set; }

        #endregion

        #region methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="primaryKey">PrimaryKey</param>
        internal GrainId(Guid primaryKey)
        {
            this.PrimaryKey = primaryKey;
        }

        /// <summary>
        /// Creates a new guid from the given value.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Guid</returns>
        internal static Guid CreateGuid(long value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        /// <summary>
        /// Creates a new guid from the given value.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Guid</returns>
        internal static Guid CreateGuid(string value)
        {
            return new Guid(value);
        }

        /// <summary>
        /// Determines whether the specified object
        /// is equals to the current object.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is GrainId))
            {
                return false;
            }

            GrainId other = obj as GrainId;
            if (this.PrimaryKey == null &&
                other.PrimaryKey == null)
            {
                return false;
            }

            return this.PrimaryKey.Equals(other.PrimaryKey);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode()
        {
            return this.PrimaryKey.GetHashCode();
        }

        #endregion
    }
}