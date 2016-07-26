//-----------------------------------------------------------------------
// <copyright file="IAsyncStream.cs">
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

namespace Orleans.Streams
{
    //
    // Summary:
    //     This interface represents an object that serves as a distributed rendevous between
    //     producers and consumers. It is similar to a Reactive Framework Subject and implements
    //     IObserver nor IObservable interfaces.
    //
    // Type parameters:
    //   T:
    //     The type of object that flows through the stream.
    public interface IAsyncStream<T> : IStreamIdentity, IEquatable<IAsyncStream<T>>, IComparable<IAsyncStream<T>>, IAsyncObservable<T>, IAsyncBatchObserver<T>, IAsyncObserver<T>
    {
        //
        // Summary:
        //     Determines whether this is a rewindable stream - supports subscribing from previous
        //     point in time.
        //
        // Returns:
        //     True if this is a rewindable stream, false otherwise.
        bool IsRewindable { get; }
        //
        // Summary:
        //     Stream Provider Name.
        string ProviderName { get; }

        //
        // Summary:
        //     Retrieves a list of all active subscriptions created by the caller for this stream.
        Task<IList<StreamSubscriptionHandle<T>>> GetAllSubscriptionHandles();
    }
}