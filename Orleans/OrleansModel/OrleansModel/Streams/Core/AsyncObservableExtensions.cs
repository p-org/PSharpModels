using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Streams
{
    public static class AsyncObservableExtensions
    {
        //
        // Summary:
        //     Subscribe a consumer to this observable using delegates. This method is a helper
        //     for the IAsyncObservable.SubscribeAsync allowing the subscribing class to inline
        //     the handler methods instead of requiring an instance of IAsyncObserver.
        //
        // Parameters:
        //   obs:
        //     The Observable object.
        //
        //   onNextAsync:
        //     Delegte that is called for IAsyncObserver.OnNextAsync.
        //
        //   onCompletedAsync:
        //     Delegte that is called for IAsyncObserver.OnCompletedAsync.
        //
        // Type parameters:
        //   T:
        //     The type of object produced by the observable.
        //
        // Returns:
        //     A promise for a StreamSubscriptionHandle that represents the subscription. The
        //     consumer may unsubscribe by using this handle. The subscription remains active
        //     for as long as it is not explicitely unsubscribed.
        public static Task<StreamSubscriptionHandle<T>> SubscribeAsync<T>(this IAsyncObservable<T> obs, Func<T, StreamSequenceToken, Task> onNextAsync, Func<Task> onCompletedAsync)
        {
            Console.WriteLine("????????????????????????????????????????????????");
            throw new NotImplementedException();
        }
    }
}
