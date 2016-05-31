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
        protected override void InitializeState()
        {
            var genericTypes = base.WrappedActorType.BaseType.GetGenericArguments();
            if (genericTypes.Length == 1)
            {
                var grainStateType = Type.GetType($"OrleansModel.GrainState`1[{genericTypes[0]}]");
                Console.WriteLine(grainStateType);
                var grainState = Activator.CreateInstance(grainStateType);
                FieldInfo field = base.WrappedActorInstance.GetType().GetField("GrainState",
                    BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(base.WrappedActorInstance, grainState);
                }

                ((IStatefulGrain)base.WrappedActorInstance).SetStorage(new InMemoryStorage());
            }
        }

        protected override void Activate()
        {
            MethodInfo mo = typeof(Grain).GetMethod("OnActivateAsync",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(base.WrappedActorInstance, new object[] { });
        }

        protected override void Deactivate()
        {
            MethodInfo mo = typeof(Grain).GetMethod("OnDeactivateAsync",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(base.WrappedActorInstance, new object[] { });
        }
    }
}
