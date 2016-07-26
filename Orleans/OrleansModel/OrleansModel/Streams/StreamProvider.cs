//-----------------------------------------------------------------------
// <copyright file="StreamProvider.cs">
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

namespace Orleans.Streams
{
    internal class StreamProvider : IStreamProvider
    {
        /// <summary>
        /// Determines whether this is a rewindable provider - supports creating rewindable
        //     streams (streams that allow subscribing from previous point in time).
        /// </summary>
        public bool IsRewindable { get; private set; }

        /// <summary>
        /// Name of the stream provider.
        /// </summary>
        public string Name { get; private set; }

        bool IStreamProvider.IsRewindable
        {
            get
            {
                return this.IsRewindable;
            }
        }

        string IStreamProvider.Name
        {
            get
            {
                return this.Name;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="isRewindable">IsRewindable</param>
        public StreamProvider(string name, bool isRewindable)
        {
            this.Name = name;
            this.IsRewindable = isRewindable;
        }

        public IAsyncStream<T> GetStream<T>(Guid streamId, string streamNamespace)
        {
            return new AsyncStream<T>(streamId, streamNamespace, this.IsRewindable);
        }
    }
}