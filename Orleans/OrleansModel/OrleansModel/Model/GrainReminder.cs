//-----------------------------------------------------------------------
// <copyright file="GrainReminder.cs">
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

using Orleans.Runtime;
using Microsoft.PSharp;
using Microsoft.PSharp.Actors;

namespace OrleansModel
{
    internal class GrainReminder : ReminderCancellationSource, IGrainReminder
    {
        /// <summary>
        /// Name of this Reminder.
        /// </summary>
        public string ReminderName
        {
            get;
            private set;
        }

        public GrainReminder(MachineId actor, MachineId reminder, string reminderName)
            : base(actor, reminder)
        {
            this.ReminderName = reminderName;
        }
    }
}
