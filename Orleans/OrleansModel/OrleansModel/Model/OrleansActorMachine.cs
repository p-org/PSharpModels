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
        protected override void Initialize(InitEvent initEvent)
        {
            Console.WriteLine(initEvent.ActorType + " >> " + initEvent.ActorType.BaseType);

            //ConstructorInfo sm = typeof(GrainState).GetConstructors().Single();
            //var stateManager = Activator.CreateInstance(typeof(ActorStateManager));
            //PropertyInfo prop = initEvent.ClassInstance.GetType().GetProperty("StateManager",
            //    BindingFlags.Public | BindingFlags.Instance);
            //if (null != prop && prop.CanWrite)
            //{
            //    prop.SetValue(initEvent.ClassInstance, stateManager, null);
            //}

            MethodInfo mo = typeof(Grain).GetMethod("OnActivateAsync",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mo.Invoke(initEvent.ClassInstance, new object[] { });
        }
    }
}
