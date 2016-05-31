//-----------------------------------------------------------------------
// <copyright file="BaseActorProxy.cs">
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.PSharp.Actors.Bridge
{
    /// <summary>
    /// The base actor proxy.
    /// </summary>
    public abstract class BaseActorProxy : Machine
    {
        /// <summary>
        /// Serializes the specified payload.
        /// </summary>
        /// <param name="payload">Payload</param>
        /// <returns>Serialized payload</returns>
        protected object[] Serialize(params object[] payload)
        {
            object[] serializedPayload = new object[payload.Length];
            for (int idx = 0; idx < payload.Length; idx++)
            {
                Type type = payload[idx].GetType();

                if (!ActorModel.Configuration.PerformSerialization ||
                    type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)))
                {
                    serializedPayload[idx] = payload[idx];
                }
                //else if (type.GetCustomAttribute(typeof(SerializableAttribute), false) != null)
                //{
                //    MemoryStream stream = new MemoryStream();
                //    BinaryFormatter bf = new BinaryFormatter();
                //    bf.Serialize(stream, payload[idx]);

                //    stream.Seek(0, SeekOrigin.Begin);
                //    serializedPayload[idx] = bf.Deserialize(stream);
                //}
                else if (type.GetInterfaces().Any(val => val == typeof(ICloneable)))
                {
                    serializedPayload[idx] = ((ICloneable)payload[idx]).Clone();
                }
                else
                {
                    try
                    {
                        serializedPayload[idx] = this.DeepClone(payload[idx]);
                    }
                    catch
                    {
                        ActorModel.Assert(false, "Payload cannot be serialized.");
                    }
                }
            }

            return serializedPayload;
        }

        /// <summary>
        /// Deep clones the specified object.
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>object</returns>
        private object DeepClone(object obj)
        {
            Type objectType = obj.GetType();
            var instance = Activator.CreateInstance(objectType);

            PropertyInfo[] propertyInfos = objectType.GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.CanWrite)
                {
                    object propertyValue = propertyInfo.GetValue(obj, null);

                    if (propertyInfo.PropertyType.IsValueType ||
                        propertyInfo.PropertyType.IsEnum ||
                        propertyInfo.PropertyType.Equals(typeof(string)))
                    {
                        propertyInfo.SetValue(instance, propertyValue, null);
                    }
                    else
                    {
                        if (propertyValue == null)
                        {
                            propertyInfo.SetValue(instance, null, null);
                        }
                        else
                        {
                            propertyInfo.SetValue(instance, this.DeepClone(propertyValue), null);
                        }
                    }
                }
            }

            return instance;
        }
    }
}
