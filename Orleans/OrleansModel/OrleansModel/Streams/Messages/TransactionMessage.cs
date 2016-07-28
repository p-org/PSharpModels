//-----------------------------------------------------------------------
// <copyright file="TransactionMessage.cs">
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
using System.Threading.Tasks;

using Microsoft.PSharp;

namespace Orleans.Streams.Messages
{
    /// <summary>
    /// Holds information about a transaction within a stream.
    /// </summary>
    [Serializable]
    public struct TransactionMessage : IEquatable<TransactionMessage>, IStreamMessage
    {
        public TransactionState State;
        public int TransactionId;

        public bool Equals(TransactionMessage other)
        {
            return other.State.Equals(this.State) && other.TransactionId == this.TransactionId;
        }

        public async Task Accept(IStreamMessageVisitor visitor)
        {
            await visitor.Visit(this);
        }
    }

    /// <summary>
    /// State of the transaction.
    /// </summary>
    public enum TransactionState
    {
        Start, End
    }
}