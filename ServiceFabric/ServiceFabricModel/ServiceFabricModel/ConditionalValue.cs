//-----------------------------------------------------------------------
// <copyright file="ConditionalValue.cs">
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

namespace Microsoft.ServiceFabric.Data
{
    //
    // Summary:
    //     Result class returned by DistributedCollections APIs that may or may not return
    //     a value.
    //
    // Type parameters:
    //   TValue:
    //     Value to initialize the result with.
    public struct ConditionalValue<TValue>
    {
        bool hasValue;
        private bool v;
        TValue value;

        public ConditionalValue(bool v) : this()
        {
            this.v = v;
        }

        //
        // Summary:
        //     Initializes a new instance of the ConditionalValue class with the given value.
        //
        // Parameters:
        //   hasValue:
        //     Indicates whether the value is valid.
        //
        //   value:
        //     The value.
        public ConditionalValue(bool hasValue, TValue value) : this()
        {
            this.hasValue = hasValue;
            this.value = value;
        }

        //
        // Summary:
        //     Gets a value indicating whether the value is valid.
        //
        // Returns:
        //     Whether the value is valid.
        public bool HasValue { get; }
        //
        // Summary:
        //     Gets the value.
        //
        // Returns:
        //     The value.
        public TValue Value { get; }
    }
}