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
using System.Linq;
using System.Reflection;

using Microsoft.PSharp.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace ServiceFabricModel
{
    public class OrleansActorMachine : ActorMachine
    {
        protected override void Initialize()
        {
            var e = this.ReceivedEvent as InitEvent;
            ConstructorInfo sm = typeof(ActorStateManager).GetConstructors().Single();
            var stateManager = Activator.CreateInstance(typeof(ActorStateManager));
            PropertyInfo prop = e.ClassInstance.GetType().GetProperty("StateManager", BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(e.ClassInstance, stateManager, null);
            }

            PropertyInfo rProp = e.ClassInstance.GetType().GetProperty("RefMachine", BindingFlags.Public | BindingFlags.Instance);
            if (null != rProp && rProp.CanWrite)
            {
                Console.WriteLine("setting ref value: " + base.RefMachine);
                rProp.SetValue(e.ClassInstance, base.RefMachine, null);
            }

            MethodInfo mo = typeof(ActorBase).GetMethod("OnActivateAsync", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(e.ClassInstance, new object[] { });
        }
    }
}
