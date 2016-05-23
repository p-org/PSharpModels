//-----------------------------------------------------------------------
// <copyright file="IStatefulGrain.cs">
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

using Orleans.Core;

namespace Orleans
{
    /// <summary>
    /// Interface for a stateful grain.
    /// </summary>
    internal interface IStatefulGrain
    {
        /// <summary>
        /// The state of this grain.
        /// </summary>
        IGrainState GrainState { get; }

        /// <summary>
        /// Sets the storage for this grain.
        /// </summary>
        /// <param name="storage">IStorage</param>
        void SetStorage(IStorage storage);
    }
}