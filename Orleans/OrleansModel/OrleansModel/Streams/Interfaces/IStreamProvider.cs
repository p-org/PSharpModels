//-----------------------------------------------------------------------
// <copyright file="IStreamProvider.cs">
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
    public interface IStreamProvider
    {
        //
        // Summary:
        //     Determines whether this is a rewindable provider - supports creating rewindable
        //     streams (streams that allow subscribing from previous point in time).
        //
        // Returns:
        //     True if this is a rewindable provider, false otherwise.
        bool IsRewindable { get; }
        //
        // Summary:
        //     Name of the stream provider.
        string Name { get; }

        IAsyncStream<T> GetStream<T>(Guid streamId, string streamNamespace);
    }
}