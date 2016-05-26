using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Microsoft.PSharp.Actors;

namespace ServiceFabricModel
{
    public class FabricActorMachine : ActorMachine
    {
        protected override void Initialize(InitEvent initEvent)
        {
            ConstructorInfo sm = typeof(ActorStateManager).GetConstructors().Single();
            var stateManager = Activator.CreateInstance(typeof(ActorStateManager));
            PropertyInfo prop = initEvent.ClassInstance.GetType().GetProperty("StateManager",
                BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(initEvent.ClassInstance, stateManager, null);
            }

            MethodInfo mo = typeof(ActorBase).GetMethod("OnActivateAsync",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(initEvent.ClassInstance, new object[] { });
        }
    }
}
