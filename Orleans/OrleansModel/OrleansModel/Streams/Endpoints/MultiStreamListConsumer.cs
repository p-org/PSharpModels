//-----------------------------------------------------------------------
// <copyright file="MultiStreamListConsumer.cs">
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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Streams.Endpoints
{
    /// <summary>
    /// Consumes items from multiple streams and places them in a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiStreamListConsumer<T> : MultiStreamConsumer<T>
    {
        public List<T> Items { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="streamProvider">Stream provider to use.</param>
        public MultiStreamListConsumer(IStreamProvider streamProvider) : base(streamProvider)
        {
            this.Items = new List<T>();
            StreamItemBatchReceivedFunc = enumerable =>
            {
                Items.AddRange(enumerable);
                return TaskDone.Done;
            };
        }
    }
}