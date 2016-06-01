//-----------------------------------------------------------------------
// <copyright file="IRemindable.cs">
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

using Orleans.Runtime;

namespace Orleans
{
    /// <summary>
    /// Callback interface that grains must implement
    /// in order to be able to register and receive
    /// Reminders.
    /// </summary>
    public interface IRemindable : IGrain, IAddressable
    {
        /// <summary>
        /// Receieve a new Reminder.
        /// </summary>
        /// <param name="reminderName">Name</param>
        /// <param name="status">TickStatus</param>
        /// <returns></returns>
        Task ReceiveReminder(string reminderName, TickStatus status);
    }
}