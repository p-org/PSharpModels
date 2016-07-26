//-----------------------------------------------------------------------
// <copyright file="StreamSubscriptionHandle.cs">
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

namespace Orleans.Streams
{
    //
    // Summary:
    //     Handle representing this subscription. Consumer may serialize and store the handle
    //     in order to unsubsribe later, for example in another activation on this grain.
    public abstract class StreamSubscriptionHandle<T> : IEquatable<StreamSubscriptionHandle<T>>
    {
        //
        // Summary:
        //     Unique identifier for this StreamSubscriptionHandle
        public abstract Guid HandleId { get; }
        public abstract IStreamIdentity StreamIdentity { get; }

        public abstract bool Equals(StreamSubscriptionHandle<T> other);
        //
        // Summary:
        //     Resumed consumption from a subscription to a stream.
        //
        // Parameters:
        //   handle:
        //     The stream handle to consume from.
        //
        // Returns:
        //     A promise with an updates subscription handle.
        public abstract Task<StreamSubscriptionHandle<T>> ResumeAsync(IAsyncObserver<T> observer, StreamSequenceToken token = null);
        //
        // Summary:
        //     Unsubscribe a stream consumer from this observable.
        //
        // Parameters:
        //   handle:
        //     The stream handle to unsubscribe.
        //
        // Returns:
        //     A promise to unsubscription action.
        public abstract Task UnsubscribeAsync();
    }
}