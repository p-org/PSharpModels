//-----------------------------------------------------------------------
// <copyright file="GrainExtensions.cs">
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
using System;

namespace Orleans
{
    /// <summary>
    /// Extension methods for grains.
    /// </summary>
    public static class GrainExtensions
    {
        public static Guid GetPrimaryKey(this IGrain grain)
        {
            return ((Grain)grain).PrimaryKey;
        }

        public static TGrainInterface AsReference<TGrainInterface>(this IAddressable grain)
        {
            if (grain == null)
            {
                throw new ArgumentNullException("grain", "Cannot pass null as an argument to AsReference");
            }

            return (TGrainInterface)grain;
        }
    }
}
