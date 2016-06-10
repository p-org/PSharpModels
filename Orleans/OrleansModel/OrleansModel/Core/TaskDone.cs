//-----------------------------------------------------------------------
// <copyright file="TaskDone.cs">
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

namespace Orleans
{
    /// <summary>
    /// A special void 'Done' Task that is already in the
    /// RunToCompletion state. Equivalent to Task.FromResult(1).
    /// </summary>
    public static class TaskDone
    {
        /// <summary>
        /// A special 'Done' Task that is already
        /// in the RunToCompletion state.
        /// </summary>
        public static Task Done
        {
            get
            {
                return Task.FromResult(1);
            }
        }
    }
}
