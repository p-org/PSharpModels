﻿//-----------------------------------------------------------------------
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
        protected override void Initialize(InitEvent initEvent)
        {
            var genericTypes = initEvent.ActorType.BaseType.GetGenericArguments();
            if (genericTypes.Length == 1)
            {
                var grainStateType = Type.GetType($"OrleansModel.GrainState`1[{genericTypes[0]}]");
                Console.WriteLine(grainStateType);
                var grainState = Activator.CreateInstance(grainStateType);
                FieldInfo field = initEvent.ClassInstance.GetType().GetField("GrainState",
                    BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(initEvent.ClassInstance, grainState);
                }

                ((IStatefulGrain)initEvent.ClassInstance).SetStorage(new InMemoryStorage());
            }
            
            MethodInfo mo = typeof(Grain).GetMethod("OnActivateAsync",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(initEvent.ClassInstance, new object[] { });
        }
    }
}
