//-----------------------------------------------------------------------
// <copyright file="IAsyncObserver.cs">
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
    //     This interface generalizes the standard .NET IObserver interface to allow asynchronous
    //     production of items.
    //     Note that this interface is implemented by item consumers and invoked (used)
    //     by item producers. This means that the producer endpoint of a stream implements
    //     this interface.
    //
    // Type parameters:
    //   T:
    //     The type of object consumed by the observer.
    public interface IAsyncObserver<in T>
    {
        //
        // Summary:
        //     Notifies the consumer that the stream was completed.
        //     The Task returned from this method should be completed when the consumer is done
        //     processing the stream closure.
        //
        // Returns:
        //     A Task that is completed when the stream-complete operation has been accepted.
        Task OnCompletedAsync();
        //
        // Summary:
        //     Notifies the consumer that the stream had an error.
        //     The Task returned from this method should be completed when the consumer is done
        //     processing the stream closure.
        //
        // Parameters:
        //   ex:
        //     An Exception that describes the error that occured on the stream.
        //
        // Returns:
        //     A Task that is completed when the close has been accepted.
        Task OnErrorAsync(Exception ex);
        //
        // Summary:
        //     Passes the next item to the consumer.
        //     The Task returned from this method should be completed when the item's processing
        //     has been sufficiently processed by the consumer to meet any behavioral guarantees.
        //     When the consumer is the (producer endpoint of) a stream, the Task is completed
        //     when the stream implementation has accepted responsibility for the item and is
        //     assured of meeting its delivery guarantees. For instance, a stream based on a
        //     durable queue would complete the Task when the item has been durably saved. A
        //     stream that provides best-effort at most once delivery would return a Task that
        //     is already complete.
        //     When the producer is the (consumer endpoint of) a stream, the Task should be
        //     completed by the consumer code when it has accepted responsibility for the item.
        //     In particular, if the stream provider guarantees at-least-once delivery, then
        //     the item should not be considered delivered until the Task returned by the consumer
        //     has been completed.
        //
        // Parameters:
        //   item:
        //     The item to be passed.
        //
        //   token:
        //     The stream sequence token of this item.
        //
        // Returns:
        //     A Task that is completed when the item has been accepted.
        Task OnNextAsync(T item, StreamSequenceToken token = null);
    }
}