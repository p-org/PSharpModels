//-----------------------------------------------------------------------
// <copyright file="GrainState.cs">
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

namespace OrleansModel
{
    /// <summary>
    /// Class implementing a grain state.
    /// </summary>
    [Serializable]
    internal class GrainState<T> : IGrainState
    {
        public T State;

        object IGrainState.State
        {
            get
            {
                return State;

            }
            set
            {
                State = (T)value;
            }
        }

        public string ETag { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GrainState()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GrainState(T state)
        {
            this.State = state;
            this.ETag = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GrainState(T state, string eTag)
        {
            this.State = state;
            this.ETag = eTag;
        }
    }
}