//-----------------------------------------------------------------------
// <copyright file="ItemMessage.cs">
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
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.PSharp;

namespace Orleans.Streams.Messages
{
    [Serializable]
    public class ItemMessage<T> : Event, IStreamMessage<T>
    {
        public IEnumerable<T> Items { get; private set; }

        public ItemMessage(IEnumerable<T> items)
            : base()
        {
            this.Items = items;
        }

        public async Task Accept(IStreamMessageVisitor<T> visitor)
        {
            await visitor.Visit(this);
        }

        public async Task Accept(IStreamMessageVisitor visitor)
        {
            var messageVisitor = visitor as IStreamMessageVisitor<T>;
            if (messageVisitor != null)
            {
                await messageVisitor.Visit(this);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}