//-----------------------------------------------------------------------
// <copyright file="OrleansActorMachine.cs">
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
using System.Reflection;

using Microsoft.PSharp.Actors;

using Orleans;

namespace OrleansModel
{
    /// <summary>
    /// An Orleans P# actor machine.
    /// </summary>
    public class OrleansActorMachine : ActorMachine
    {
        protected override void Initialize()
        {
            var e = this.ReceivedEvent as InitEvent;

            //var stateManager = Activator.CreateInstance(typeof(ActorStateManager));
            //PropertyInfo prop = e.ClassInstance.GetType().GetProperty(
            //    "StateManager", BindingFlags.Public | BindingFlags.Instance);
            //if (null != prop && prop.CanWrite)
            //{
            //    prop.SetValue(e.ClassInstance, stateManager, null);
            //}

            PropertyInfo refMachine = e.ClassInstance.GetType().GetProperty(
                "RefMachine", BindingFlags.Public | BindingFlags.Instance);
            if (null != refMachine && refMachine.CanWrite)
            {
                refMachine.SetValue(e.ClassInstance, base.RefMachine, null);
            }

            MethodInfo mo = typeof(Grain).GetMethod("OnActivateAsync",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(e.ClassInstance, new object[] { });
        }
    }
}
