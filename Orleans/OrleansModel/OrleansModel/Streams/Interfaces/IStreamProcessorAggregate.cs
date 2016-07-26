//-----------------------------------------------------------------------
// <copyright file="IStreamProcessorAggregate.cs">
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

namespace Orleans.Streams
{
    /// <summary>
    /// Transforms data from TIn to TOut.
    /// </summary>
    /// <typeparam name="TIn">Data input type.</typeparam>
    /// <typeparam name="TOut">Data output type.</typeparam>
    public interface IStreamProcessorAggregate<TIn, TOut> : IGrainWithGuidKey,
        ITransactionalStreamConsumerAggregate<TIn>, ITransactionalStreamProviderAggregate<TOut>
    {
    }
}