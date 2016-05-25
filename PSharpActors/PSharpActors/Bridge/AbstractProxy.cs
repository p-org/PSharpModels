//-----------------------------------------------------------------------
// <copyright file="AbstractProxy.cs">
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

namespace Microsoft.PSharp.Actors.Bridge
{
    public class AbstractProxy
    {
        public TResult GetResult<TResult>(Task<TResult> task)
        {
            return ((ActorCompletionTask<TResult>)task).Result;
        }

        public void Wait<TResult>(Task<TResult> task)
        {
            ((ActorCompletionTask<TResult>)task).Wait();
        }

        public void Wait(Task task)
        {
            ((ActorCompletionTask<object>)task).Wait();
        }
    }
}
