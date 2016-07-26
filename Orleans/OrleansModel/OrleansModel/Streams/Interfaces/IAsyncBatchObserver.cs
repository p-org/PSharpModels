//-----------------------------------------------------------------------
// <copyright file="IAsyncBatchObserver.cs">
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

namespace Orleans.Streams
{
    //
    // Summary:
    //     This interface generalizes the IAsyncObserver interface to allow production and
    //     consumption of batches of items.
    //     Note that this interface is implemented by item consumers and invoked (used)
    //     by item producers. This means that the producer endpoint of a stream implements
    //     this interface.
    //
    // Type parameters:
    //   T:
    //     The type of object consumed by the observer.
    public interface IAsyncBatchObserver<in T> : IAsyncObserver<T>
    {
        //
        // Summary:
        //     Passes the next batch of items to the consumer.
        //     The Task returned from this method should be completed when all items in the
        //     batch have been sufficiently processed by the consumer to meet any behavioral
        //     guarantees.
        //     That is, the semantics of the returned Task is the same as for OnNextAsync, extended
        //     for all items in the batch.
        //
        // Parameters:
        //   batch:
        //     The items to be passed.
        //
        //   token:
        //     The stream sequence token of this batch of items.
        //
        // Returns:
        //     A Task that is completed when the batch has been accepted.
        Task OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken token = null);
    }
}