//-----------------------------------------------------------------------
// <copyright file="ITransactionalStreamTearDown.cs">
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
    /// <summary>
    /// Supports a tear down operation to dispose resources.
    /// </summary>
    public interface ITransactionalStreamTearDown
    {
        /// <summary>
        /// Unsubscribes from all streams this entity consumes and remove references to stream this entity produces.
        /// </summary>
        /// <returns></returns>
        Task TearDown();

        /// <summary>
        /// Checks if this entity is teared down.
        /// </summary>
        /// <returns></returns>
        Task<bool> IsTearedDown();
    }
}