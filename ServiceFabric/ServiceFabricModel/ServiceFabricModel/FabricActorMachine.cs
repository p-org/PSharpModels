//-----------------------------------------------------------------------
// <copyright file="FabricActorMachine.cs">
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
    /// <summary>
    /// A Service Fabric P# actor machine.
    /// </summary>
    public class FabricActorMachine : ActorMachine
    {
        //protected override void Initialize(InitEvent initEvent)
        //{
        //    ConstructorInfo sm = typeof(ActorStateManager).GetConstructors().Single();
        //    var stateManager = Activator.CreateInstance(typeof(ActorStateManager));
        //    PropertyInfo prop = initEvent.ClassInstance.GetType().GetProperty("StateManager",
        //        BindingFlags.Public | BindingFlags.Instance);
        //    if (null != prop && prop.CanWrite)
        //    {
        //        prop.SetValue(initEvent.ClassInstance, stateManager, null);
        //    }

        //    MethodInfo mo = typeof(ActorBase).GetMethod("OnActivateAsync",
        //        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    mo.Invoke(initEvent.ClassInstance, new object[] { });
        //}

        protected override void Initialize()
        {
            ConstructorInfo sm = typeof(ActorStateManager).GetConstructors().Single();
            var stateManager = Activator.CreateInstance(typeof(ActorStateManager));
            PropertyInfo prop = base.WrappedActorType.GetProperty("StateManager",
                BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(base.WrappedActorInstance, stateManager, null);
            }

        }

        protected override void Activate()
        {
            MethodInfo mo = typeof(ActorBase).GetMethod("OnActivateAsync",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(base.WrappedActorInstance, new object[] { });
        }

        protected override void Deactivate()
        {
            MethodInfo mo = typeof(ActorBase).GetMethod("OnDeactivateAsync",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(base.WrappedActorInstance, new object[] { });
        }
    }
}
