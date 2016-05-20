﻿using Microsoft.ServiceFabric.Actors.Runtime;
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
        protected override void Initialize()
        {
            var e = this.ReceivedEvent as InitEvent;
            ConstructorInfo sm = typeof(ActorStateManager).GetConstructors().Single();
            var stateManager = Activator.CreateInstance(typeof(ActorStateManager));
            PropertyInfo prop = e.classInstance.GetType().GetProperty("StateManager", BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(e.classInstance, stateManager, null);
            }

            PropertyInfo rProp = e.classInstance.GetType().GetProperty("refMachine", BindingFlags.Public | BindingFlags.Instance);
            if (null != rProp && rProp.CanWrite)
            {
                Console.WriteLine("setting ref value: " + base.RefMachine);
                rProp.SetValue(e.classInstance, base.RefMachine, null);
            }

            MethodInfo mo = typeof(ActorBase).GetMethod("OnActivateAsync", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(e.classInstance, new object[] { });
        }
    }
}