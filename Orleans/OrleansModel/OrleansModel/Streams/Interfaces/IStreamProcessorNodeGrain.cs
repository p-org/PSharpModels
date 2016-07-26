//-----------------------------------------------------------------------
// <copyright file="IAsyncObservable.cs">
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

using System.Threading.Tasks;

namespace Orleans.Streams
{
    //
    // Summary:
    //     This interface generalizes the standard .NET IObserveable interface to allow
    //     asynchronous consumption of items. Asynchronous here means that the consumer
    //     can process items asynchronously and signal item completion to the producer by
    //     completing the returned Task.
    //     Note that this interface is invoked (used) by item consumers and implemented
    //     by item producers. This means that the producer endpoint of a stream implements
    //     this interface.
    //
    // Type parameters:
    //   T:
    //     The type of object produced by the observable.
    public interface IAsyncObservable<T>
    {
        //
        // Summary:
        //     Subscribe a consumer to this observable.
        //
        // Parameters:
        //   observer:
        //     The asynchronous observer to subscribe.
        //
        // Returns:
        //     A promise for a StreamSubscriptionHandle that represents the subscription. The
        //     consumer may unsubscribe by using this handle. The subscription remains active
        //     for as long as it is not explicitely unsubscribed.
        Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer);
        //
        // Summary:
        //     Subscribe a consumer to this observable.
        //
        // Parameters:
        //   observer:
        //     The asynchronous observer to subscribe.
        //
        //   token:
        //     The stream sequence to be used as an offset to start the subscription from.
        //
        //   filterFunc:
        //     Filter to be applied for this subscription
        //
        //   filterData:
        //     Data object that will be passed in to the filterFunc. This will usually contain
        //     any paramaters required by the filterFunc to make it's filtering decision.
        //
        // Returns:
        //     A promise for a StreamSubscriptionHandle that represents the subscription. The
        //     consumer may unsubscribe by using this handle. The subscription remains active
        //     for as long as it is not explicitely unsubscribed.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Thrown if the supplied stream filter function is not suitable. Usually this is
        //     because it is not a static method.
        Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer, StreamSequenceToken token, StreamFilterPredicate filterFunc = null, object filterData = null);
    }
}