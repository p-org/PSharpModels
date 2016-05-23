//-----------------------------------------------------------------------
// <copyright file="DummyTask.cs">
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
using System.Threading.Tasks;

namespace Microsoft.PSharp.Actors.Bridge
{
    public class DummyTask : Task
    {
        public DummyTask()
            : base(() => { })
        {

        }

        public static Task<TResult> FromResult<TResult>(Func<TResult> function) 
        {
            return new Task<TResult>(function);
        }
    }

    public class DummyTask<T> : System.Threading.Tasks.Task<T>
    {
        private new T Result;

        public DummyTask()
            : base(() => { return default(T); })
        {

        }

        public DummyTask(Func<T> function)
            : base(function)
        {
            this.Result = function();
        }
    }
}
